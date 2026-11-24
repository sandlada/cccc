using System.Threading;
using System.Threading.Tasks;
using Cccc.Application;
using Xunit;

namespace Cccc.Application.Tests
{
    public sealed class FeedbackUseCaseTests
    {
        private static PreferencesService Preferences(bool showPreview = true)
        {
            var store = new MemoryPreferencesStore
            {
                LoadResult = new()
                {
                    State = PreferencesLoadState.Loaded,
                    Model = new PreferencesModel { ShowPreview = showPreview }
                }
            };
            var service = new PreferencesService(store, new MemoryPreferencesStore());
            service.Load();
            return service;
        }

        [Fact]
        public void Record_copy_attempt_uses_foreground_baseline_and_preview_preference()
        {
            var session = new FeedbackSession();
            var clock = new FakeSystemClock { CurrentTimeMilliseconds = 100 };
            var foreground = new FakeForegroundWindowQuery();
            var clipboard = new FakeClipboardReader { Sequence = 42 };
            var presenter = new RecordingFeedbackPresenter();
            var handler = new RecordCopyAttemptHandler(session, clock, foreground, clipboard,
                presenter, Preferences(false));

            var result = handler.Handle();

            Assert.Equal(1, result.Count);
            Assert.Equal(7, result.Source.Handle);
            Assert.Equal(9u, result.Source.ProcessId);
            Assert.NotNull(presenter.Copy);
            Assert.Equal(1, presenter.Copy.Count);
            Assert.False(presenter.Copy.ShowPreview);
            Assert.Equal(42u, clipboard.CurrentSequence);
        }

        [Fact]
        public async Task Observe_clipboard_reads_expected_sequence_and_presents_confirmation()
        {
            var session = new FeedbackSession();
            session.RecordCopy(100, new WindowTarget(7, 9), 42);
            var clock = new FakeSystemClock { CurrentTimeMilliseconds = 1100 };
            var foreground = new FakeForegroundWindowQuery();
            var clipboard = new FakeClipboardReader
            {
                Sequence = 42,
                Result = new()
                {
                    Sequence = 43,
                    OwnerProcess = 9,
                    Supported = true,
                    Preview = "hello",
                    TypeLabel = "text"
                }
            };
            var presenter = new RecordingFeedbackPresenter();
            var handler = new ObserveClipboardHandler(session, clock, clipboard, foreground, presenter);

            FeedbackView view = await handler.HandleAsync(100, CancellationToken.None);

            Assert.NotNull(view);
            Assert.Equal(FeedbackViewKind.Confirmed, view.Kind);
            Assert.Equal(42u, clipboard.ExpectedSequence);
            Assert.Equal(1600, clipboard.Deadline);
            Assert.NotNull(presenter.Presented);
            Assert.Equal("hello", presenter.Presented.Preview);
        }

        [Fact]
        public async Task Observe_clipboard_discards_result_after_generation_reset()
        {
            var session = new FeedbackSession();
            session.RecordCopy(100, new WindowTarget(7, 9), 42);
            var clock = new FakeSystemClock { CurrentTimeMilliseconds = 1100 };
            var foreground = new FakeForegroundWindowQuery();
            var completion = new TaskCompletionSource<ClipboardContent>();
            var clipboard = new FakeClipboardReader { Sequence = 42, Pending = completion };
            var presenter = new RecordingFeedbackPresenter();
            var handler = new ObserveClipboardHandler(session, clock, clipboard, foreground, presenter);

            Task<FeedbackView> pending = handler.HandleAsync(100, CancellationToken.None);
            session.Reset(42);
            completion.SetResult(new()
            {
                Sequence = 43,
                OwnerProcess = 9,
                Supported = true
            });

            Assert.Null(await pending);
            Assert.Null(presenter.Presented);
        }

        [Fact]
        public async Task Observe_clipboard_discards_result_after_confirmation_deadline()
        {
            var session = new FeedbackSession();
            session.RecordCopy(100, new WindowTarget(7, 9), 42);
            var clock = new FakeSystemClock { CurrentTimeMilliseconds = 1700 };
            var foreground = new FakeForegroundWindowQuery();
            var clipboard = new FakeClipboardReader
            {
                Sequence = 42,
                Result = new()
                {
                    Sequence = 43,
                    OwnerProcess = 9,
                    Supported = true
                }
            };
            var presenter = new RecordingFeedbackPresenter();
            var handler = new ObserveClipboardHandler(session, clock, clipboard, foreground, presenter);

            Assert.Null(await handler.HandleAsync(100, CancellationToken.None));
            Assert.Null(presenter.Presented);
        }

        [Fact]
        public void Expire_reports_unconfirmed_when_no_result_arrived()
        {
            var session = new FeedbackSession();
            session.RecordCopy(100, new WindowTarget(7, 9), 42);
            var clock = new FakeSystemClock { CurrentTimeMilliseconds = 1600 };
            var clipboard = new FakeClipboardReader { Sequence = 42 };
            var presenter = new RecordingFeedbackPresenter();
            var handler = new ExpireAttemptHandler(session, clock, clipboard, presenter);

            FeedbackView view = handler.Handle();

            Assert.NotNull(view);
            Assert.Equal(FeedbackViewKind.Unconfirmed, view.Kind);
            Assert.Equal(1, presenter.Presented.Count);
        }

        [Fact]
        public void Set_paused_resets_session_and_presents_status()
        {
            var session = new FeedbackSession();
            var clipboard = new FakeClipboardReader { Sequence = 42 };
            var foreground = new FakeForegroundWindowQuery();
            var presenter = new RecordingFeedbackPresenter();
            var handler = new SetPausedHandler(session, clipboard, foreground, presenter);

            SetPausedResult result = handler.Handle(true);

            Assert.True(result.Paused);
            Assert.True(session.Paused);
            Assert.Equal(1, presenter.HideCount);
            Assert.Equal(FeedbackViewKind.Status, presenter.Presented.Kind);
            Assert.Equal("Cccc 已暂停", presenter.Presented.Title);
        }

        [Fact]
        public void Change_preferences_updates_model_and_persists()
        {
            var store = new MemoryPreferencesStore
            {
                LoadResult = new()
                {
                    State = PreferencesLoadState.Loaded,
                    Model = new PreferencesModel()
                }
            };
            var service = new PreferencesService(store, new MemoryPreferencesStore());
            service.Load();
            var handler = new ChangePreferencesHandler(service);

            ChangePreferencesResult result = handler.Handle(new()
            {
                ShowPreview = false,
                UseMaterial = false,
                Layout = LayoutMode.Taskbar
            });

            Assert.True(result.Success);
            Assert.False(result.Preferences.ShowPreview);
            Assert.False(result.Preferences.UseMaterial);
            Assert.Equal(LayoutMode.Taskbar, result.Preferences.Layout);
            Assert.Equal(1, store.SaveCount);
        }

        [Fact]
        public async Task Overflow_count_is_formatted_in_signal_and_feedback_view()
        {
            var session = new FeedbackSession();
            var clock = new FakeSystemClock { CurrentTimeMilliseconds = 100 };
            var foreground = new FakeForegroundWindowQuery();
            var clipboard = new FakeClipboardReader
            {
                Sequence = 1,
                Result = new()
                {
                    Sequence = 2,
                    OwnerProcess = 9,
                    Supported = true,
                    Preview = "text",
                    TypeLabel = "text"
                }
            };
            var presenter = new RecordingFeedbackPresenter();
            var prefs = Preferences(false);
            var recordHandler = new RecordCopyAttemptHandler(session, clock, foreground, clipboard, presenter, prefs);

            for (int i = 0; i < 100; i++)
            {
                clock.CurrentTimeMilliseconds += 10;
                recordHandler.Handle();
            }

            Assert.NotNull(presenter.Copy);
            Assert.Equal(100, presenter.Copy.Count);
            Assert.Equal("99+", presenter.Copy.CountText);

            var observeHandler = new ObserveClipboardHandler(session, clock, clipboard, foreground, presenter);
            FeedbackView view = await observeHandler.HandleAsync(clock.CurrentTimeMilliseconds, CancellationToken.None);

            Assert.NotNull(view);
            Assert.Equal(100, view.Count);
            Assert.Equal("99+", view.CountText);
            Assert.Equal("99+", presenter.Presented.CountText);
        }
    }
}
