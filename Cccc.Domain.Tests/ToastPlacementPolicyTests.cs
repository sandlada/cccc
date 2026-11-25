using Cccc.Domain.Preferences;
using Xunit;

namespace Cccc.Domain.Tests
{
    public sealed class ToastPlacementPolicyTests
    {
        private static readonly ScreenRect WorkArea = new(0, 0, 1920, 1032);
        private const double CardWidth = 360;
        private const double CardHeight = 80;

        [Theory]
        [InlineData(ToastPosition.TopLeft, 20, 20)]
        [InlineData(ToastPosition.TopCenter, 780, 20)]
        [InlineData(ToastPosition.TopRight, 1540, 20)]
        [InlineData(ToastPosition.MiddleLeft, 20, 476)]
        [InlineData(ToastPosition.MiddleRight, 1540, 476)]
        [InlineData(ToastPosition.BottomLeft, 20, 932)]
        [InlineData(ToastPosition.BottomCenter, 780, 932)]
        [InlineData(ToastPosition.BottomRight, 1540, 932)]
        public void Eight_base_positions_compute_expected_card_top_left_at_scale_one(
            ToastPosition position, double expectedX, double expectedY)
        {
            ScreenPoint point = ToastPlacementPolicy.ComputeCardTopLeft(
                position, WorkArea, CardWidth, CardHeight, 0, 0, 1.0);

            Assert.Equal(expectedX, point.X);
            Assert.Equal(expectedY, point.Y);
        }

        [Fact]
        public void Offsets_shift_position_by_scaled_dip_and_clamp_to_work_area()
        {
            ScreenPoint shifted = ToastPlacementPolicy.ComputeCardTopLeft(
                ToastPosition.BottomLeft, WorkArea, CardWidth, CardHeight, 50, -30, 1.5);

            // Base at 1.5x: margin = 30; X = 30 + 75 = 105; Y = 1032 - 30 - 80 - 45 = 877
            Assert.Equal(105, shifted.X);
            Assert.Equal(877, shifted.Y);

            // Large negative offset clamps to work area origin
            ScreenPoint clamped = ToastPlacementPolicy.ComputeCardTopLeft(
                ToastPosition.TopLeft, WorkArea, CardWidth, CardHeight, -999, -999, 1.0);
            Assert.Equal(0, clamped.X);
            Assert.Equal(0, clamped.Y);

            // Large positive offset clamps to work area max minus card size
            ScreenPoint clampedMax = ToastPlacementPolicy.ComputeCardTopLeft(
                ToastPosition.BottomRight, WorkArea, CardWidth, CardHeight, 999, 999, 1.0);
            Assert.Equal(1920 - CardWidth, clampedMax.X);
            Assert.Equal(1032 - CardHeight, clampedMax.Y);
        }

        [Theory]
        [InlineData(-1000, -400)]
        [InlineData(-400, -400)]
        [InlineData(0, 0)]
        [InlineData(400, 400)]
        [InlineData(1000, 400)]
        public void Offset_clamps_within_domain_bounds(int input, int expected)
        {
            Assert.Equal(expected, ToastPlacementPolicy.ClampOffset(input));
        }

        [Theory]
        [InlineData(ToastPosition.TopLeft, 0, -1)]
        [InlineData(ToastPosition.TopCenter, 0, -1)]
        [InlineData(ToastPosition.TopRight, 0, -1)]
        [InlineData(ToastPosition.MiddleLeft, -1, 0)]
        [InlineData(ToastPosition.MiddleRight, 1, 0)]
        [InlineData(ToastPosition.BottomLeft, 0, 1)]
        [InlineData(ToastPosition.BottomCenter, 0, 1)]
        [InlineData(ToastPosition.BottomRight, 0, 1)]
        public void Entrance_slide_vector_points_to_nearest_edge(
            ToastPosition position, double expectedX, double expectedY)
        {
            ScreenPoint vec = ToastPlacementPolicy.GetEntranceSlideVector(position);
            Assert.Equal(expectedX, vec.X);
            Assert.Equal(expectedY, vec.Y);
        }
    }
}
