function Ae(e,t,r,i){return`${e}: ${t}${r}${i?";":""}`}function _e(e,t,r){return`var(${e}, ${t}${r})`}function $e(e,t){return`${e}${t}`}var Sr=class{Key;Value;Unit;toJSON(){return{Key:this.Key,Value:this.Value,Unit:this.Unit}}toString(){return this.ToCSSDeclaration()}ToCSSDeclaration({Semicolon:e=!1}={}){return Ae(this.Key,this.Value,this.Unit,e)}ToCSSVariable(){return _e(this.Key,this.Value,this.Unit)}ToCSSValue(){return $e(this.Value,this.Unit)}constructor(e,t,r){this.Key=e,this.Value=t,this.Unit=r}};var ot=class dt extends Sr{static DraggedStateLayerOpacity=new dt("--md-sys-state-dragged-state-layer-opacity",.16,"");static PressedStateLayerOpacity=new dt("--md-sys-state-pressed-state-layer-opacity",.1,"");static FocusedStateLayerOpacity=new dt("--md-sys-state-focused-state-layer-opacity",.1,"");static HoveredStateLayerOpacity=new dt("--md-sys-state-hovered-state-layer-opacity",.08,"");static DisabledStateLayerOpacity=new dt("--md-sys-state-disabled-state-layer-opacity",.38,"");static FocusIndicator={Thickness:new dt("--md-sys-state-focus-indicator-thickness",3,"px"),OuterOffset:new dt("--md-sys-state-focus-indicator-outer-offset",2,"px")};static AllEnums={DraggedStateLayerOpacity:this.DraggedStateLayerOpacity,PressedStateLayerOpacity:this.PressedStateLayerOpacity,FocusedStateLayerOpacity:this.FocusedStateLayerOpacity,HoveredStateLayerOpacity:this.HoveredStateLayerOpacity,DisabledStateLayerOpacity:this.DisabledStateLayerOpacity,FocusIndicator:{Thickness:this.FocusIndicator.Thickness,OuterOffset:this.FocusIndicator.OuterOffset}}};var vr={PrimaryKeyColor:{Key:"--md-sys-color-primary-key-color",Value:"#039d54"},SecondaryKeyColor:{Key:"--md-sys-color-secondary-key-color",Value:"#54815f"},TertiaryKeyColor:{Key:"--md-sys-color-tertiary-key-color",Value:"#ffa97f"},ErrorKeyColor:{Key:"--md-sys-color-error-key-color",Value:"#de3730"},NeutralKeyColor:{Key:"--md-sys-color-neutral-key-color",Value:"#727971"},NeutralVariantKeyColor:{Key:"--md-sys-color-neutral-variant-key-color",Value:"#6d7a6e"},Background:{Key:"--md-sys-color-background",Value:"light-dark(#f4fbf1, #0e150f)"},Error:{Key:"--md-sys-color-error",Value:"light-dark(#ba1a1a, #ffb4ab)"},ErrorContainer:{Key:"--md-sys-color-error-container",Value:"light-dark(#ffdad6, #93000a)"},ErrorDim:{Key:"--md-sys-color-error-dim",Value:"light-dark(#a80710, #ff554a)"},InverseOnSurface:{Key:"--md-sys-color-inverse-on-surface",Value:"light-dark(#ecf3e9, #2b322c)"},InversePrimary:{Key:"--md-sys-color-inverse-primary",Value:"light-dark(#61de8c, #006d38)"},InverseSurface:{Key:"--md-sys-color-inverse-surface",Value:"light-dark(#2b322c, #dde4db)"},OnBackground:{Key:"--md-sys-color-on-background",Value:"light-dark(#171d17, #dde4db)"},OnError:{Key:"--md-sys-color-on-error",Value:"light-dark(#ffffff, #690005)"},OnErrorContainer:{Key:"--md-sys-color-on-error-container",Value:"light-dark(#93000a, #ffdad6)"},OnPrimary:{Key:"--md-sys-color-on-primary",Value:"light-dark(#ffffff, #00391a)"},OnPrimaryContainer:{Key:"--md-sys-color-on-primary-container",Value:"light-dark(#00592c, #00592c)"},OnPrimaryFixed:{Key:"--md-sys-color-on-primary-fixed",Value:"light-dark(#00210d, #00210d)"},OnPrimaryFixedVariant:{Key:"--md-sys-color-on-primary-fixed-variant",Value:"light-dark(#005229, #005229)"},OnSecondary:{Key:"--md-sys-color-on-secondary",Value:"light-dark(#ffffff, #0a381c)"},OnSecondaryContainer:{Key:"--md-sys-color-on-secondary-container",Value:"light-dark(#406c4b, #97c7a0)"},OnSecondaryFixed:{Key:"--md-sys-color-on-secondary-fixed",Value:"light-dark(#00210d, #00210d)"},OnSecondaryFixedVariant:{Key:"--md-sys-color-on-secondary-fixed-variant",Value:"light-dark(#244f31, #244f31)"},OnSurface:{Key:"--md-sys-color-on-surface",Value:"light-dark(#171d17, #dde4db)"},OnSurfaceVariant:{Key:"--md-sys-color-on-surface-variant",Value:"light-dark(#3d4a3f, #bccabc)"},OnTertiary:{Key:"--md-sys-color-on-tertiary",Value:"light-dark(#ffffff, #552002)"},OnTertiaryContainer:{Key:"--md-sys-color-on-tertiary-container",Value:"light-dark(#793c1a, #793c1a)"},OnTertiaryFixed:{Key:"--md-sys-color-on-tertiary-fixed",Value:"light-dark(#351000, #351000)"},OnTertiaryFixedVariant:{Key:"--md-sys-color-on-tertiary-fixed-variant",Value:"light-dark(#723615, #723615)"},Outline:{Key:"--md-sys-color-outline",Value:"light-dark(#6d7a6e, #879487)"},OutlineVariant:{Key:"--md-sys-color-outline-variant",Value:"light-dark(#bccabc, #3d4a3f)"},Primary:{Key:"--md-sys-color-primary",Value:"light-dark(#006d38, #76f29e)"},PrimaryContainer:{Key:"--md-sys-color-primary-container",Value:"light-dark(#58d584, #58d584)"},PrimaryDim:{Key:"--md-sys-color-primary-dim",Value:"light-dark(#006030, #82fea8)"},PrimaryFixed:{Key:"--md-sys-color-primary-fixed",Value:"light-dark(#7ffba6, #7ffba6)"},PrimaryFixedDim:{Key:"--md-sys-color-primary-fixed-dim",Value:"light-dark(#61de8c, #61de8c)"},Scrim:{Key:"--md-sys-color-scrim",Value:"light-dark(#000000, #000000)"},Secondary:{Key:"--md-sys-color-secondary",Value:"light-dark(#3c6847, #a2d2ab)"},SecondaryContainer:{Key:"--md-sys-color-secondary-container",Value:"light-dark(#bbecc3, #295436)"},SecondaryDim:{Key:"--md-sys-color-secondary-dim",Value:"light-dark(#305b3c, #bdeec5)"},SecondaryFixed:{Key:"--md-sys-color-secondary-fixed",Value:"light-dark(#bdeec5, #bdeec5)"},SecondaryFixedDim:{Key:"--md-sys-color-secondary-fixed-dim",Value:"light-dark(#a2d2ab, #a2d2ab)"},Shadow:{Key:"--md-sys-color-shadow",Value:"light-dark(#000000, #000000)"},Surface:{Key:"--md-sys-color-surface",Value:"light-dark(#f4fbf1, #0e150f)"},SurfaceBright:{Key:"--md-sys-color-surface-bright",Value:"light-dark(#f4fbf1, #343b34)"},SurfaceContainer:{Key:"--md-sys-color-surface-container",Value:"light-dark(#e9f0e6, #1b211b)"},SurfaceContainerHigh:{Key:"--md-sys-color-surface-container-high",Value:"light-dark(#e3eae1, #252c25)"},SurfaceContainerHighest:{Key:"--md-sys-color-surface-container-highest",Value:"light-dark(#dde4db, #303630)"},SurfaceContainerLow:{Key:"--md-sys-color-surface-container-low",Value:"light-dark(#eff6ec, #171d17)"},SurfaceContainerLowest:{Key:"--md-sys-color-surface-container-lowest",Value:"light-dark(#ffffff, #09100a)"},SurfaceDim:{Key:"--md-sys-color-surface-dim",Value:"light-dark(#d5dcd3, #0e150f)"},SurfaceTint:{Key:"--md-sys-color-surface-tint",Value:"light-dark(#006d38, #61de8c)"},SurfaceVariant:{Key:"--md-sys-color-surface-variant",Value:"light-dark(#d8e6d7, #3d4a3f)"},Tertiary:{Key:"--md-sys-color-tertiary",Value:"light-dark(#8f4d2a, #ffcfba)"},TertiaryContainer:{Key:"--md-sys-color-tertiary-container",Value:"light-dark(#ffa97f, #ffa97f)"},TertiaryDim:{Key:"--md-sys-color-tertiary-dim",Value:"light-dark(#804120, #fda77d)"},TertiaryFixed:{Key:"--md-sys-color-tertiary-fixed",Value:"light-dark(#ffdbcc, #ffdbcc)"},TertiaryFixedDim:{Key:"--md-sys-color-tertiary-fixed-dim",Value:"light-dark(#ffb693, #ffb693)"}};var wi=class{Key;Value;toJSON(){return{Key:this.Key,Value:this.Value}}toString(){return this.Value}ToCSSDeclaration({Semicolon:e=!1}={}){return Ae(this.Key,this.Value,"",e)}ToCSSVariable(){return _e(this.Key,this.Value,"")}ToCSSValue(){return $e(this.Value,"")}constructor(e,t){this.Key=e,this.Value=t}},xr=class Cr{contract;constructor(t){this.contract=t}static From(t={}){let r={...vr,...t},i=Object.fromEntries(Object.entries(r).map(([o,n])=>[o,new wi(n.Key,n.Value)]));return new Cr(i)}get PrimaryKeyColor(){return this.contract.PrimaryKeyColor}get SecondaryKeyColor(){return this.contract.SecondaryKeyColor}get TertiaryKeyColor(){return this.contract.TertiaryKeyColor}get ErrorKeyColor(){return this.contract.ErrorKeyColor}get NeutralKeyColor(){return this.contract.NeutralKeyColor}get NeutralVariantKeyColor(){return this.contract.NeutralVariantKeyColor}get Background(){return this.contract.Background}get Error(){return this.contract.Error}get ErrorContainer(){return this.contract.ErrorContainer}get ErrorDim(){return this.contract.ErrorDim}get InverseOnSurface(){return this.contract.InverseOnSurface}get InversePrimary(){return this.contract.InversePrimary}get InverseSurface(){return this.contract.InverseSurface}get OnBackground(){return this.contract.OnBackground}get OnError(){return this.contract.OnError}get OnErrorContainer(){return this.contract.OnErrorContainer}get OnPrimary(){return this.contract.OnPrimary}get OnPrimaryContainer(){return this.contract.OnPrimaryContainer}get OnPrimaryFixed(){return this.contract.OnPrimaryFixed}get OnPrimaryFixedVariant(){return this.contract.OnPrimaryFixedVariant}get OnSecondary(){return this.contract.OnSecondary}get OnSecondaryContainer(){return this.contract.OnSecondaryContainer}get OnSecondaryFixed(){return this.contract.OnSecondaryFixed}get OnSecondaryFixedVariant(){return this.contract.OnSecondaryFixedVariant}get OnSurface(){return this.contract.OnSurface}get OnSurfaceVariant(){return this.contract.OnSurfaceVariant}get OnTertiary(){return this.contract.OnTertiary}get OnTertiaryContainer(){return this.contract.OnTertiaryContainer}get OnTertiaryFixed(){return this.contract.OnTertiaryFixed}get OnTertiaryFixedVariant(){return this.contract.OnTertiaryFixedVariant}get Outline(){return this.contract.Outline}get OutlineVariant(){return this.contract.OutlineVariant}get Primary(){return this.contract.Primary}get PrimaryContainer(){return this.contract.PrimaryContainer}get PrimaryDim(){return this.contract.PrimaryDim}get PrimaryFixed(){return this.contract.PrimaryFixed}get PrimaryFixedDim(){return this.contract.PrimaryFixedDim}get Scrim(){return this.contract.Scrim}get Secondary(){return this.contract.Secondary}get SecondaryContainer(){return this.contract.SecondaryContainer}get SecondaryDim(){return this.contract.SecondaryDim}get SecondaryFixed(){return this.contract.SecondaryFixed}get SecondaryFixedDim(){return this.contract.SecondaryFixedDim}get Shadow(){return this.contract.Shadow}get Surface(){return this.contract.Surface}get SurfaceBright(){return this.contract.SurfaceBright}get SurfaceContainer(){return this.contract.SurfaceContainer}get SurfaceContainerHigh(){return this.contract.SurfaceContainerHigh}get SurfaceContainerHighest(){return this.contract.SurfaceContainerHighest}get SurfaceContainerLow(){return this.contract.SurfaceContainerLow}get SurfaceContainerLowest(){return this.contract.SurfaceContainerLowest}get SurfaceDim(){return this.contract.SurfaceDim}get SurfaceTint(){return this.contract.SurfaceTint}get SurfaceVariant(){return this.contract.SurfaceVariant}get Tertiary(){return this.contract.Tertiary}get TertiaryContainer(){return this.contract.TertiaryContainer}get TertiaryDim(){return this.contract.TertiaryDim}get TertiaryFixed(){return this.contract.TertiaryFixed}get TertiaryFixedDim(){return this.contract.TertiaryFixedDim}get AllEnums(){return{PrimaryKeyColor:this.PrimaryKeyColor,SecondaryKeyColor:this.SecondaryKeyColor,TertiaryKeyColor:this.TertiaryKeyColor,ErrorKeyColor:this.ErrorKeyColor,NeutralKeyColor:this.NeutralKeyColor,NeutralVariantKeyColor:this.NeutralVariantKeyColor,Background:this.Background,Error:this.Error,ErrorContainer:this.ErrorContainer,ErrorDim:this.ErrorDim,InverseOnSurface:this.InverseOnSurface,InversePrimary:this.InversePrimary,InverseSurface:this.InverseSurface,OnBackground:this.OnBackground,OnError:this.OnError,OnErrorContainer:this.OnErrorContainer,OnPrimary:this.OnPrimary,OnPrimaryContainer:this.OnPrimaryContainer,OnPrimaryFixed:this.OnPrimaryFixed,OnPrimaryFixedVariant:this.OnPrimaryFixedVariant,OnSecondary:this.OnSecondary,OnSecondaryContainer:this.OnSecondaryContainer,OnSecondaryFixed:this.OnSecondaryFixed,OnSecondaryFixedVariant:this.OnSecondaryFixedVariant,OnSurface:this.OnSurface,OnSurfaceVariant:this.OnSurfaceVariant,OnTertiary:this.OnTertiary,OnTertiaryContainer:this.OnTertiaryContainer,OnTertiaryFixed:this.OnTertiaryFixed,OnTertiaryFixedVariant:this.OnTertiaryFixedVariant,Outline:this.Outline,OutlineVariant:this.OutlineVariant,Primary:this.Primary,PrimaryContainer:this.PrimaryContainer,PrimaryDim:this.PrimaryDim,PrimaryFixed:this.PrimaryFixed,PrimaryFixedDim:this.PrimaryFixedDim,Scrim:this.Scrim,Secondary:this.Secondary,SecondaryContainer:this.SecondaryContainer,SecondaryDim:this.SecondaryDim,SecondaryFixed:this.SecondaryFixed,SecondaryFixedDim:this.SecondaryFixedDim,Shadow:this.Shadow,Surface:this.Surface,SurfaceBright:this.SurfaceBright,SurfaceContainer:this.SurfaceContainer,SurfaceContainerHigh:this.SurfaceContainerHigh,SurfaceContainerHighest:this.SurfaceContainerHighest,SurfaceContainerLow:this.SurfaceContainerLow,SurfaceContainerLowest:this.SurfaceContainerLowest,SurfaceDim:this.SurfaceDim,SurfaceTint:this.SurfaceTint,SurfaceVariant:this.SurfaceVariant,Tertiary:this.Tertiary,TertiaryContainer:this.TertiaryContainer,TertiaryDim:this.TertiaryDim,TertiaryFixed:this.TertiaryFixed,TertiaryFixedDim:this.TertiaryFixedDim}}};var E=xr.From();var Ei=e=>{if(typeof e!="object"||e===null)return!1;let t=Object.getPrototypeOf(e);return t===Object.prototype||t===null},Pi=e=>{if(typeof e!="object"||e===null)return!1;let t=e;return typeof t.ToCSSVariable=="function"||"_$cssResult$"in t||"cssText"in t};function Kt(e){let t={};for(let[o,n]of Object.entries(e??{}))if(n!=null){if(Array.isArray(n)){t[o]=Object.freeze([...n]);continue}if(Ei(n)&&!Pi(n)){let s={};for(let[l,a]of Object.entries(n))a!=null&&(s[l]=a);t[o]=Object.freeze(s);continue}t[o]=n}let r=Object.freeze(Object.keys(t)),i={...t};return Object.defineProperties(i,{__brand:{value:"ResolvedStyleDefinition",enumerable:!1,writable:!1,configurable:!1},tokens:{value:Object.freeze({...t}),enumerable:!1,writable:!1,configurable:!1},flatTokenKeys:{value:r,enumerable:!1,writable:!1,configurable:!1}}),Object.freeze(i)}var re=globalThis,ie=re.ShadowRoot&&(re.ShadyCSS===void 0||re.ShadyCSS.nativeShadow)&&"adoptedStyleSheets"in Document.prototype&&"replace"in CSSStyleSheet.prototype,Me=Symbol(),wr=new WeakMap,qt=class{constructor(t,r,i){if(this._$cssResult$=!0,i!==Me)throw Error("CSSResult is not constructable. Use `unsafeCSS` or `css` instead.");this.cssText=t,this.t=r}get styleSheet(){let t=this.o,r=this.t;if(ie&&t===void 0){let i=r!==void 0&&r.length===1;i&&(t=wr.get(r)),t===void 0&&((this.o=t=new CSSStyleSheet).replaceSync(this.cssText),i&&wr.set(r,t))}return t}toString(){return this.cssText}},ut=e=>new qt(typeof e=="string"?e:e+"",void 0,Me),Ct=(e,...t)=>{let r=e.length===1?e[0]:t.reduce((i,o,n)=>i+(s=>{if(s._$cssResult$===!0)return s.cssText;if(typeof s=="number")return s;throw Error("Value passed to 'css' function must be a 'css' function result: "+s+". Use 'unsafeCSS' to pass non-literal values, but take care to ensure page security.")})(o)+e[n+1],e[0]);return new qt(r,e,Me)},Er=(e,t)=>{if(ie)e.adoptedStyleSheets=t.map(r=>r instanceof CSSStyleSheet?r:r.styleSheet);else for(let r of t){let i=document.createElement("style"),o=re.litNonce;o!==void 0&&i.setAttribute("nonce",o),i.textContent=r.cssText,e.appendChild(i)}},ke=ie?e=>e:e=>e instanceof CSSStyleSheet?(t=>{let r="";for(let i of t.cssRules)r+=i.cssText;return ut(r)})(e):e;var{is:Ai,defineProperty:_i,getOwnPropertyDescriptor:$i,getOwnPropertyNames:Mi,getOwnPropertySymbols:ki,getPrototypeOf:Ri}=Object,oe=globalThis,Pr=oe.trustedTypes,Oi=Pr?Pr.emptyScript:"",Ti=oe.reactiveElementPolyfillSupport,Bt=(e,t)=>e,Xt={toAttribute(e,t){switch(t){case Boolean:e=e?Oi:null;break;case Object:case Array:e=e==null?e:JSON.stringify(e)}return e},fromAttribute(e,t){let r=e;switch(t){case Boolean:r=e!==null;break;case Number:r=e===null?null:Number(e);break;case Object:case Array:try{r=JSON.parse(e)}catch{r=null}}return r}},ne=(e,t)=>!Ai(e,t),Ar={attribute:!0,type:String,converter:Xt,reflect:!1,useDefault:!1,hasChanged:ne};Symbol.metadata??=Symbol("metadata"),oe.litPropertyMetadata??=new WeakMap;var nt=class extends HTMLElement{static addInitializer(t){this._$Ei(),(this.l??=[]).push(t)}static get observedAttributes(){return this.finalize(),this._$Eh&&[...this._$Eh.keys()]}static createProperty(t,r=Ar){if(r.state&&(r.attribute=!1),this._$Ei(),this.prototype.hasOwnProperty(t)&&((r=Object.create(r)).wrapped=!0),this.elementProperties.set(t,r),!r.noAccessor){let i=Symbol(),o=this.getPropertyDescriptor(t,i,r);o!==void 0&&_i(this.prototype,t,o)}}static getPropertyDescriptor(t,r,i){let{get:o,set:n}=$i(this.prototype,t)??{get(){return this[r]},set(s){this[r]=s}};return{get:o,set(s){let l=o?.call(this);n?.call(this,s),this.requestUpdate(t,l,i)},configurable:!0,enumerable:!0}}static getPropertyOptions(t){return this.elementProperties.get(t)??Ar}static _$Ei(){if(this.hasOwnProperty(Bt("elementProperties")))return;let t=Ri(this);t.finalize(),t.l!==void 0&&(this.l=[...t.l]),this.elementProperties=new Map(t.elementProperties)}static finalize(){if(this.hasOwnProperty(Bt("finalized")))return;if(this.finalized=!0,this._$Ei(),this.hasOwnProperty(Bt("properties"))){let r=this.properties,i=[...Mi(r),...ki(r)];for(let o of i)this.createProperty(o,r[o])}let t=this[Symbol.metadata];if(t!==null){let r=litPropertyMetadata.get(t);if(r!==void 0)for(let[i,o]of r)this.elementProperties.set(i,o)}this._$Eh=new Map;for(let[r,i]of this.elementProperties){let o=this._$Eu(r,i);o!==void 0&&this._$Eh.set(o,r)}this.elementStyles=this.finalizeStyles(this.styles)}static finalizeStyles(t){let r=[];if(Array.isArray(t)){let i=new Set(t.flat(1/0).reverse());for(let o of i)r.unshift(ke(o))}else t!==void 0&&r.push(ke(t));return r}static _$Eu(t,r){let i=r.attribute;return i===!1?void 0:typeof i=="string"?i:typeof t=="string"?t.toLowerCase():void 0}constructor(){super(),this._$Ep=void 0,this.isUpdatePending=!1,this.hasUpdated=!1,this._$Em=null,this._$Ev()}_$Ev(){this._$ES=new Promise(t=>this.enableUpdating=t),this._$AL=new Map,this._$E_(),this.requestUpdate(),this.constructor.l?.forEach(t=>t(this))}addController(t){(this._$EO??=new Set).add(t),this.renderRoot!==void 0&&this.isConnected&&t.hostConnected?.()}removeController(t){this._$EO?.delete(t)}_$E_(){let t=new Map,r=this.constructor.elementProperties;for(let i of r.keys())this.hasOwnProperty(i)&&(t.set(i,this[i]),delete this[i]);t.size>0&&(this._$Ep=t)}createRenderRoot(){let t=this.shadowRoot??this.attachShadow(this.constructor.shadowRootOptions);return Er(t,this.constructor.elementStyles),t}connectedCallback(){this.renderRoot??=this.createRenderRoot(),this.enableUpdating(!0),this._$EO?.forEach(t=>t.hostConnected?.())}enableUpdating(t){}disconnectedCallback(){this._$EO?.forEach(t=>t.hostDisconnected?.())}attributeChangedCallback(t,r,i){this._$AK(t,i)}_$ET(t,r){let i=this.constructor.elementProperties.get(t),o=this.constructor._$Eu(t,i);if(o!==void 0&&i.reflect===!0){let n=(i.converter?.toAttribute!==void 0?i.converter:Xt).toAttribute(r,i.type);this._$Em=t,n==null?this.removeAttribute(o):this.setAttribute(o,n),this._$Em=null}}_$AK(t,r){let i=this.constructor,o=i._$Eh.get(t);if(o!==void 0&&this._$Em!==o){let n=i.getPropertyOptions(o),s=typeof n.converter=="function"?{fromAttribute:n.converter}:n.converter?.fromAttribute!==void 0?n.converter:Xt;this._$Em=o;let l=s.fromAttribute(r,n.type);this[o]=l??this._$Ej?.get(o)??l,this._$Em=null}}requestUpdate(t,r,i,o=!1,n){if(t!==void 0){let s=this.constructor;if(o===!1&&(n=this[t]),i??=s.getPropertyOptions(t),!((i.hasChanged??ne)(n,r)||i.useDefault&&i.reflect&&n===this._$Ej?.get(t)&&!this.hasAttribute(s._$Eu(t,i))))return;this.C(t,r,i)}this.isUpdatePending===!1&&(this._$ES=this._$EP())}C(t,r,{useDefault:i,reflect:o,wrapped:n},s){i&&!(this._$Ej??=new Map).has(t)&&(this._$Ej.set(t,s??r??this[t]),n!==!0||s!==void 0)||(this._$AL.has(t)||(this.hasUpdated||i||(r=void 0),this._$AL.set(t,r)),o===!0&&this._$Em!==t&&(this._$Eq??=new Set).add(t))}async _$EP(){this.isUpdatePending=!0;try{await this._$ES}catch(r){Promise.reject(r)}let t=this.scheduleUpdate();return t!=null&&await t,!this.isUpdatePending}scheduleUpdate(){return this.performUpdate()}performUpdate(){if(!this.isUpdatePending)return;if(!this.hasUpdated){if(this.renderRoot??=this.createRenderRoot(),this._$Ep){for(let[o,n]of this._$Ep)this[o]=n;this._$Ep=void 0}let i=this.constructor.elementProperties;if(i.size>0)for(let[o,n]of i){let{wrapped:s}=n,l=this[o];s!==!0||this._$AL.has(o)||l===void 0||this.C(o,void 0,n,l)}}let t=!1,r=this._$AL;try{t=this.shouldUpdate(r),t?(this.willUpdate(r),this._$EO?.forEach(i=>i.hostUpdate?.()),this.update(r)):this._$EM()}catch(i){throw t=!1,this._$EM(),i}t&&this._$AE(r)}willUpdate(t){}_$AE(t){this._$EO?.forEach(r=>r.hostUpdated?.()),this.hasUpdated||(this.hasUpdated=!0,this.firstUpdated(t)),this.updated(t)}_$EM(){this._$AL=new Map,this.isUpdatePending=!1}get updateComplete(){return this.getUpdateComplete()}getUpdateComplete(){return this._$ES}shouldUpdate(t){return!0}update(t){this._$Eq&&=this._$Eq.forEach(r=>this._$ET(r,this[r])),this._$EM()}updated(t){}firstUpdated(t){}};nt.elementStyles=[],nt.shadowRootOptions={mode:"open"},nt[Bt("elementProperties")]=new Map,nt[Bt("finalized")]=new Map,Ti?.({ReactiveElement:nt}),(oe.reactiveElementVersions??=[]).push("2.1.2");var Ve=globalThis,_r=e=>e,se=Ve.trustedTypes,$r=se?se.createPolicy("lit-html",{createHTML:e=>e}):void 0,Fr="$lit$",pt=`lit$${Math.random().toFixed(9).slice(2)}$`,Dr="?"+pt,Fi=`<${Dr}>`,Pt=document,Yt=()=>Pt.createComment(""),Ht=e=>e===null||typeof e!="object"&&typeof e!="function",ze=Array.isArray,Di=e=>ze(e)||typeof e?.[Symbol.iterator]=="function",Re=`[ 	
\f\r]`,Nt=/<(?:(!--|\/[^a-zA-Z])|(\/?[a-zA-Z][^>\s]*)|(\/?$))/g,Mr=/-->/g,kr=/>/g,wt=RegExp(`>|${Re}(?:([^\\s"'>=/]+)(${Re}*=${Re}*(?:[^ 	
\f\r"'\`<>=]|("|')|))|$)`,"g"),Rr=/'/g,Or=/"/g,Lr=/^(?:script|style|textarea|title)$/i,Ie=e=>(t,...r)=>({_$litType$:e,strings:t,values:r}),V=Ie(1),Vn=Ie(2),zn=Ie(3),Y=Symbol.for("lit-noChange"),M=Symbol.for("lit-nothing"),Tr=new WeakMap,Et=Pt.createTreeWalker(Pt,129);function Vr(e,t){if(!ze(e)||!e.hasOwnProperty("raw"))throw Error("invalid template strings array");return $r!==void 0?$r.createHTML(t):t}var Li=(e,t)=>{let r=e.length-1,i=[],o,n=t===2?"<svg>":t===3?"<math>":"",s=Nt;for(let l=0;l<r;l++){let a=e[l],c,d,h=-1,v=0;for(;v<a.length&&(s.lastIndex=v,d=s.exec(a),d!==null);)v=s.lastIndex,s===Nt?d[1]==="!--"?s=Mr:d[1]!==void 0?s=kr:d[2]!==void 0?(Lr.test(d[2])&&(o=RegExp("</"+d[2],"g")),s=wt):d[3]!==void 0&&(s=wt):s===wt?d[0]===">"?(s=o??Nt,h=-1):d[1]===void 0?h=-2:(h=s.lastIndex-d[2].length,c=d[1],s=d[3]===void 0?wt:d[3]==='"'?Or:Rr):s===Or||s===Rr?s=wt:s===Mr||s===kr?s=Nt:(s=wt,o=void 0);let u=s===wt&&e[l+1].startsWith("/>")?" ":"";n+=s===Nt?a+Fi:h>=0?(i.push(c),a.slice(0,h)+Fr+a.slice(h)+pt+u):a+pt+(h===-2?l:u)}return[Vr(e,n+(e[r]||"<?>")+(t===2?"</svg>":t===3?"</math>":"")),i]},Ut=class e{constructor({strings:t,_$litType$:r},i){let o;this.parts=[];let n=0,s=0,l=t.length-1,a=this.parts,[c,d]=Li(t,r);if(this.el=e.createElement(c,i),Et.currentNode=this.el.content,r===2||r===3){let h=this.el.content.firstChild;h.replaceWith(...h.childNodes)}for(;(o=Et.nextNode())!==null&&a.length<l;){if(o.nodeType===1){if(o.hasAttributes())for(let h of o.getAttributeNames())if(h.endsWith(Fr)){let v=d[s++],u=o.getAttribute(h).split(pt),b=/([.?@])?(.*)/.exec(v);a.push({type:1,index:n,name:b[2],strings:u,ctor:b[1]==="."?Te:b[1]==="?"?Fe:b[1]==="@"?De:Tt}),o.removeAttribute(h)}else h.startsWith(pt)&&(a.push({type:6,index:n}),o.removeAttribute(h));if(Lr.test(o.tagName)){let h=o.textContent.split(pt),v=h.length-1;if(v>0){o.textContent=se?se.emptyScript:"";for(let u=0;u<v;u++)o.append(h[u],Yt()),Et.nextNode(),a.push({type:2,index:++n});o.append(h[v],Yt())}}}else if(o.nodeType===8)if(o.data===Dr)a.push({type:2,index:n});else{let h=-1;for(;(h=o.data.indexOf(pt,h+1))!==-1;)a.push({type:7,index:n}),h+=pt.length-1}n++}}static createElement(t,r){let i=Pt.createElement("template");return i.innerHTML=t,i}};function Ot(e,t,r=e,i){if(t===Y)return t;let o=i!==void 0?r._$Co?.[i]:r._$Cl,n=Ht(t)?void 0:t._$litDirective$;return o?.constructor!==n&&(o?._$AO?.(!1),n===void 0?o=void 0:(o=new n(e),o._$AT(e,r,i)),i!==void 0?(r._$Co??=[])[i]=o:r._$Cl=o),o!==void 0&&(t=Ot(e,o._$AS(e,t.values),o,i)),t}var Oe=class{constructor(t,r){this._$AV=[],this._$AN=void 0,this._$AD=t,this._$AM=r}get parentNode(){return this._$AM.parentNode}get _$AU(){return this._$AM._$AU}u(t){let{el:{content:r},parts:i}=this._$AD,o=(t?.creationScope??Pt).importNode(r,!0);Et.currentNode=o;let n=Et.nextNode(),s=0,l=0,a=i[0];for(;a!==void 0;){if(s===a.index){let c;a.type===2?c=new Gt(n,n.nextSibling,this,t):a.type===1?c=new a.ctor(n,a.name,a.strings,this,t):a.type===6&&(c=new Le(n,this,t)),this._$AV.push(c),a=i[++l]}s!==a?.index&&(n=Et.nextNode(),s++)}return Et.currentNode=Pt,o}p(t){let r=0;for(let i of this._$AV)i!==void 0&&(i.strings!==void 0?(i._$AI(t,i,r),r+=i.strings.length-2):i._$AI(t[r])),r++}},Gt=class e{get _$AU(){return this._$AM?._$AU??this._$Cv}constructor(t,r,i,o){this.type=2,this._$AH=M,this._$AN=void 0,this._$AA=t,this._$AB=r,this._$AM=i,this.options=o,this._$Cv=o?.isConnected??!0}get parentNode(){let t=this._$AA.parentNode,r=this._$AM;return r!==void 0&&t?.nodeType===11&&(t=r.parentNode),t}get startNode(){return this._$AA}get endNode(){return this._$AB}_$AI(t,r=this){t=Ot(this,t,r),Ht(t)?t===M||t==null||t===""?(this._$AH!==M&&this._$AR(),this._$AH=M):t!==this._$AH&&t!==Y&&this._(t):t._$litType$!==void 0?this.$(t):t.nodeType!==void 0?this.T(t):Di(t)?this.k(t):this._(t)}O(t){return this._$AA.parentNode.insertBefore(t,this._$AB)}T(t){this._$AH!==t&&(this._$AR(),this._$AH=this.O(t))}_(t){this._$AH!==M&&Ht(this._$AH)?this._$AA.nextSibling.data=t:this.T(Pt.createTextNode(t)),this._$AH=t}$(t){let{values:r,_$litType$:i}=t,o=typeof i=="number"?this._$AC(t):(i.el===void 0&&(i.el=Ut.createElement(Vr(i.h,i.h[0]),this.options)),i);if(this._$AH?._$AD===o)this._$AH.p(r);else{let n=new Oe(o,this),s=n.u(this.options);n.p(r),this.T(s),this._$AH=n}}_$AC(t){let r=Tr.get(t.strings);return r===void 0&&Tr.set(t.strings,r=new Ut(t)),r}k(t){ze(this._$AH)||(this._$AH=[],this._$AR());let r=this._$AH,i,o=0;for(let n of t)o===r.length?r.push(i=new e(this.O(Yt()),this.O(Yt()),this,this.options)):i=r[o],i._$AI(n),o++;o<r.length&&(this._$AR(i&&i._$AB.nextSibling,o),r.length=o)}_$AR(t=this._$AA.nextSibling,r){for(this._$AP?.(!1,!0,r);t!==this._$AB;){let i=_r(t).nextSibling;_r(t).remove(),t=i}}setConnected(t){this._$AM===void 0&&(this._$Cv=t,this._$AP?.(t))}},Tt=class{get tagName(){return this.element.tagName}get _$AU(){return this._$AM._$AU}constructor(t,r,i,o,n){this.type=1,this._$AH=M,this._$AN=void 0,this.element=t,this.name=r,this._$AM=o,this.options=n,i.length>2||i[0]!==""||i[1]!==""?(this._$AH=Array(i.length-1).fill(new String),this.strings=i):this._$AH=M}_$AI(t,r=this,i,o){let n=this.strings,s=!1;if(n===void 0)t=Ot(this,t,r,0),s=!Ht(t)||t!==this._$AH&&t!==Y,s&&(this._$AH=t);else{let l=t,a,c;for(t=n[0],a=0;a<n.length-1;a++)c=Ot(this,l[i+a],r,a),c===Y&&(c=this._$AH[a]),s||=!Ht(c)||c!==this._$AH[a],c===M?t=M:t!==M&&(t+=(c??"")+n[a+1]),this._$AH[a]=c}s&&!o&&this.j(t)}j(t){t===M?this.element.removeAttribute(this.name):this.element.setAttribute(this.name,t??"")}},Te=class extends Tt{constructor(){super(...arguments),this.type=3}j(t){this.element[this.name]=t===M?void 0:t}},Fe=class extends Tt{constructor(){super(...arguments),this.type=4}j(t){this.element.toggleAttribute(this.name,!!t&&t!==M)}},De=class extends Tt{constructor(t,r,i,o,n){super(t,r,i,o,n),this.type=5}_$AI(t,r=this){if((t=Ot(this,t,r,0)??M)===Y)return;let i=this._$AH,o=t===M&&i!==M||t.capture!==i.capture||t.once!==i.once||t.passive!==i.passive,n=t!==M&&(i===M||o);o&&this.element.removeEventListener(this.name,this,i),n&&this.element.addEventListener(this.name,this,t),this._$AH=t}handleEvent(t){typeof this._$AH=="function"?this._$AH.call(this.options?.host??this.element,t):this._$AH.handleEvent(t)}},Le=class{constructor(t,r,i){this.element=t,this.type=6,this._$AN=void 0,this._$AM=r,this.options=i}get _$AU(){return this._$AM._$AU}_$AI(t){Ot(this,t)}};var Vi=Ve.litHtmlPolyfillSupport;Vi?.(Ut,Gt),(Ve.litHtmlVersions??=[]).push("3.3.3");var zr=(e,t,r)=>{let i=r?.renderBefore??t,o=i._$litPart$;if(o===void 0){let n=r?.renderBefore??null;i._$litPart$=o=new Gt(t.insertBefore(Yt(),n),n,void 0,r??{})}return o._$AI(e),o};var je=globalThis,Q=class extends nt{constructor(){super(...arguments),this.renderOptions={host:this},this._$Do=void 0}createRenderRoot(){let t=super.createRenderRoot();return this.renderOptions.renderBefore??=t.firstChild,t}update(t){let r=this.render();this.hasUpdated||(this.renderOptions.isConnected=this.isConnected),super.update(t),this._$Do=zr(r,this.renderRoot,this.renderOptions)}connectedCallback(){super.connectedCallback(),this._$Do?.setConnected(!0)}disconnectedCallback(){super.disconnectedCallback(),this._$Do?.setConnected(!1)}render(){return Y}};Q._$litElement$=!0,Q.finalized=!0,je.litElementHydrateSupport?.({LitElement:Q});var zi=je.litElementPolyfillSupport;zi?.({LitElement:Q});(je.litElementVersions??=[]).push("4.2.2");var Ir=e=>{let t=e.trim();return(t.startsWith("--")?t:`--${t}`).replace(/-+$/,"")},jr=(e,t)=>e.length===0?`--${t}`:`${e}-${t}`,Ii=e=>{if(typeof e=="string")return{prefix:Ir(e),includePublicVars:!0};let{prefix:t="",includePublicVars:r=!0,selector:i}=e??{};return{prefix:Ir(t),includePublicVars:r,selector:i}},Ke=e=>{if(e==null)return"";if(typeof e=="object"&&e!==null){if(typeof e.ToCSSVariable=="function")return e.ToCSSVariable();if("cssText"in e&&typeof e.cssText=="string")return e.cssText;if("rawVal"in e)return Ke(e.rawVal)}return String(e)},Kr=e=>{if(typeof e!="object"||e===null)return!1;let t=Object.getPrototypeOf(e);return t===Object.prototype||t===null},qr=e=>{if(typeof e!="object"||e===null)return!1;let t=e;return typeof t.ToCSSVariable=="function"||"_$cssResult$"in t||"cssText"in t||"rawVal"in t},qe=e=>{if(Array.isArray(e))return!0;if(typeof e!="object"||e===null)return!1;let t=e;return qr(e)?"rawVal"in t?qe(t.rawVal):!1:Kr(e)?Object.values(t).some(qe):!1},ji=new Set(["__brand","schema","tokens","flatTokenKeys","forwardedBridges"]);function Wt(e){let t=Ii(e);return r=>{if(!r||typeof r!="object")return ut("");let i="tokens"in r&&typeof r.tokens=="object"&&r.tokens!==null?r.tokens:r,o=[];for(let[n,s]of Object.entries(i)){if(s==null||ji.has(n))continue;let l=n.startsWith("--")?n.slice(2):n;if(qe(s))throw new Error(`[stringifyTokens] Token '${l}' uses an array but the simplified style system holds no schema: only static values and plain records are allowed.`);if(Kr(s)&&!qr(s)){for(let[c,d]of Object.entries(s))if(d!=null){let h=Ke(d),v=`${c}-${l}`;t.includePublicVars?o.push(`--_${v}: var(${jr(t.prefix,v)}, ${h});`):o.push(`--_${v}: ${h};`)}continue}let a=Ke(s);t.includePublicVars?o.push(`--_${l}: var(${jr(t.prefix,l)}, ${a});`):o.push(`--_${l}: ${a};`)}if(o.length===0)return ut("");if(t.selector&&t.selector.trim().length>0){let n=o.map(s=>`    ${s}`).join(`
`);return ut(`${t.selector} {
${n}
}`)}return ut(o.join(`
`))}}var st={"extra-small":24,small:36,medium:48,large:72,"extra-large":96},Be=Kt({"container-size":{"extra-small":`${st["extra-small"]}px`,small:`${st.small}px`,medium:`${st.medium}px`,large:`${st.large}px`,"extra-large":`${st["extra-large"]}px`},"outline-width":"1px","focus-outline-width":ot.FocusIndicator.Thickness,"focus-outline-color":E.Secondary,"elevated-shadow-offset-y":"2px","elevated-shadow-blur":"6px","elevated-shadow-color":`color-mix(in srgb, ${E.Shadow} 22%, transparent)`,"enabled-filled-container-color":E.Primary,"enabled-filled-content-color":E.OnPrimary,"enabled-tonal-container-color":E.PrimaryContainer,"enabled-tonal-content-color":E.OnPrimaryContainer,"enabled-outlined-container-color":"transparent","enabled-outlined-content-color":E.OnSurface,"enabled-outlined-outline-color":E.OutlineVariant,"enabled-elevated-container-color":E.SurfaceContainerLow,"enabled-elevated-content-color":E.Primary,"enabled-surface-container-color":E.SurfaceContainerHighest,"enabled-surface-content-color":E.OnSurface,"enabled-transparent-container-color":"transparent","enabled-transparent-content-color":E.OnSurface,"enabled-filled-container-color-secondary":E.Secondary,"enabled-filled-content-color-secondary":E.OnSecondary,"enabled-tonal-container-color-secondary":E.SecondaryContainer,"enabled-tonal-content-color-secondary":E.OnSecondaryContainer,"enabled-outlined-outline-color-secondary":E.Secondary,"enabled-elevated-content-color-secondary":E.Secondary,"enabled-filled-container-color-tertiary":E.Tertiary,"enabled-filled-content-color-tertiary":E.OnTertiary,"enabled-tonal-container-color-tertiary":E.TertiaryContainer,"enabled-tonal-content-color-tertiary":E.OnTertiaryContainer,"enabled-outlined-outline-color-tertiary":E.Tertiary,"enabled-elevated-content-color-tertiary":E.Tertiary,"enabled-filled-container-color-error":E.Error,"enabled-filled-content-color-error":E.OnError,"enabled-tonal-container-color-error":E.ErrorContainer,"enabled-tonal-content-color-error":E.OnErrorContainer,"enabled-outlined-outline-color-error":E.Error,"enabled-elevated-content-color-error":E.Error,"enabled-filled-container-color-surface":E.InverseSurface,"enabled-filled-content-color-surface":E.InverseOnSurface,"enabled-tonal-container-color-surface":E.SurfaceContainerHigh,"enabled-tonal-content-color-surface":E.OnSurface,"enabled-outlined-outline-color-surface":E.Outline,"enabled-elevated-content-color-surface":E.OnSurface,"hovered-state-layer-opacity":ot.HoveredStateLayerOpacity,"focused-state-layer-opacity":ot.FocusedStateLayerOpacity,"pressed-state-layer-opacity":ot.PressedStateLayerOpacity,"disabled-container-opacity":"0.12","disabled-content-opacity":"0.38"});var y={Circle:"circle",Square:"square",Slanted:"slanted",Arch:"arch",Fan:"fan",Arrow:"arrow",SemiCircle:"semi-circle",Oval:"oval",Pill:"pill",Triangle:"triangle",Diamond:"diamond",ClamShell:"clam-shell",Pentagon:"pentagon",Gem:"gem",Sunny:"sunny",VerySunny:"very-sunny",Cookie4Sided:"cookie-4-sided",Cookie6Sided:"cookie-6-sided",Cookie7Sided:"cookie-7-sided",Cookie9Sided:"cookie-9-sided",Cookie12Sided:"cookie-12-sided",Ghostish:"ghostish",Clover4Leaf:"clover-4-leaf",Clover8Leaf:"clover-8-leaf",Burst:"burst",SoftBurst:"soft-burst",Boom:"boom",SoftBoom:"soft-boom",Flower:"flower",Puffy:"puffy",PuffyDiamond:"puffy-diamond",PixelCircle:"pixel-circle",PixelTriangle:"pixel-triangle",Bun:"bun",Heart:"heart"},H={Polygon:"polygon",Star:"star",Rectangle:"rectangle",PillStar:"pill-star"},Ki={...y,...H},qi={Filled:"filled",Tonal:"tonal",Outlined:"outlined",Elevated:"elevated",Surface:"surface",Transparent:"transparent"},Bi={Primary:"primary",Secondary:"secondary",Tertiary:"tertiary",Error:"error",Surface:"surface"},Xi={ExtraSmall:"extra-small",Small:"small",Medium:"medium",Large:"large",ExtraLarge:"extra-large"},Ni={Contain:"contain",Fill:"fill"},Yi={Expressive:"expressive",Standard:"standard"},Hi={Fast:"fast",Default:"default",Slow:"slow"},Xe="shape-morph-start",Ne="shape-morph-end",Ye="shape-change";var g=(e,t)=>({x:e,y:t});var Ft=(e,t)=>Math.hypot(e,t),ae=(e,t)=>e*e+t*t,le=e=>e*e,At=(e,t)=>{let r=Ft(e,t);if(!(r>0))throw new Error("Required distance greater than zero");return g(e/r,t/r)},Ui=e=>g(Math.cos(e),Math.sin(e)),ft=(e,t,r=g(0,0))=>{let i=Ui(t);return g(i.x*e+r.x,i.y*e+r.y)},_t=e=>g(-e.y,e.x),Z=(e,t)=>g(e.x-t.x,e.y-t.y),D=(e,t)=>g(e.x+t.x,e.y+t.y),K=(e,t)=>g(e.x*t,e.y*t),yt=(e,t)=>e.x*t.x+e.y*t.y,gt=(e,t)=>Ft(e.x-t.x,e.y-t.y),Br=(e,t)=>ae(e.x-t.x,e.y-t.y),Gi=(e,t)=>e.x*t.y-e.y*t.x>0,j=(e,t,r)=>(1-r)*e+r*t,Xr=(e,t,r)=>g(j(e.x,t.x,r),j(e.y,t.y,r)),B=(e,t)=>(e%t+t)%t,ce=e=>e/360*2*Math.PI,Nr=e=>Math.atan2(e.y,e.x)*180/Math.PI,$t=(e,t,r=g(0,0))=>{let i=ce(t),o=Z(e,r),n=Math.cos(i),s=Math.sin(i);return D(g(o.x*n-o.y*s,o.x*s+o.y*n),r)},he=(e,t,r,i,o,n,s=1e-4)=>{let l=_t(g(r-e,i-t)),a=g(o-e,n-t),c=Math.abs(yt(l,a)),d=s*Ft(l.x,l.y)*Ft(a.x,a.y);return c<s||c<d},Jt=(e,t,r)=>Gi(Z(t,e),Z(r,t)),Dt=(e,t)=>{let r=Math.abs(e-t);return Math.min(r,1-r)},He=(e,t,r)=>r>=t?e>=t&&e<=r:e>=t||e<=r;var Yr=e=>Math.abs(e)<1e-4,z=class at{constructor(t){if(t.length!==8)throw new Error("Points array size should be 8");this.points=t}get anchor0X(){return this.points[0]}get anchor0Y(){return this.points[1]}get control0X(){return this.points[2]}get control0Y(){return this.points[3]}get control1X(){return this.points[4]}get control1Y(){return this.points[5]}get anchor1X(){return this.points[6]}get anchor1Y(){return this.points[7]}pointOnCurve(t){let r=1-t;return g(this.anchor0X*(r*r*r)+this.control0X*(3*t*r*r)+this.control1X*(3*t*t*r)+this.anchor1X*(t*t*t),this.anchor0Y*(r*r*r)+this.control0Y*(3*t*r*r)+this.control1Y*(3*t*t*r)+this.anchor1Y*(t*t*t))}zeroLength(){return Math.abs(this.anchor0X-this.anchor1X)<1e-4&&Math.abs(this.anchor0Y-this.anchor1Y)<1e-4}calculateBounds(t,r=!1){if(this.zeroLength()){t[0]=this.anchor0X,t[1]=this.anchor0Y,t[2]=this.anchor0X,t[3]=this.anchor0Y;return}let i=Math.min(this.anchor0X,this.anchor1X),o=Math.min(this.anchor0Y,this.anchor1Y),n=Math.max(this.anchor0X,this.anchor1X),s=Math.max(this.anchor0Y,this.anchor1Y);if(r){t[0]=Math.min(i,Math.min(this.control0X,this.control1X)),t[1]=Math.min(o,Math.min(this.control0Y,this.control1Y)),t[2]=Math.max(n,Math.max(this.control0X,this.control1X)),t[3]=Math.max(s,Math.max(this.control0Y,this.control1Y));return}let l=-this.anchor0X+3*this.control0X-3*this.control1X+this.anchor1X,a=2*this.anchor0X-4*this.control0X+2*this.control1X,c=-this.anchor0X+this.control0X;if(Yr(l)){if(a!==0){let u=2*c/(-2*a);if(u>=0&&u<=1){let b=this.pointOnCurve(u).x;b<i&&(i=b),b>n&&(n=b)}}}else{let u=a*a-4*l*c;if(u>=0){let b=(-a+Math.sqrt(u))/(2*l);if(b>=0&&b<=1){let x=this.pointOnCurve(b).x;x<i&&(i=x),x>n&&(n=x)}let f=(-a-Math.sqrt(u))/(2*l);if(f>=0&&f<=1){let x=this.pointOnCurve(f).x;x<i&&(i=x),x>n&&(n=x)}}}let d=-this.anchor0Y+3*this.control0Y-3*this.control1Y+this.anchor1Y,h=2*this.anchor0Y-4*this.control0Y+2*this.control1Y,v=-this.anchor0Y+this.control0Y;if(Yr(d)){if(h!==0){let u=2*v/(-2*h);if(u>=0&&u<=1){let b=this.pointOnCurve(u).y;b<o&&(o=b),b>s&&(s=b)}}}else{let u=h*h-4*d*v;if(u>=0){let b=(-h+Math.sqrt(u))/(2*d);if(b>=0&&b<=1){let x=this.pointOnCurve(b).y;x<o&&(o=x),x>s&&(s=x)}let f=(-h-Math.sqrt(u))/(2*d);if(f>=0&&f<=1){let x=this.pointOnCurve(f).y;x<o&&(o=x),x>s&&(s=x)}}}t[0]=i,t[1]=o,t[2]=n,t[3]=s}split(t){let r=1-t,i=this.pointOnCurve(t);return[new at(new Float64Array([this.anchor0X,this.anchor0Y,this.anchor0X*r+this.control0X*t,this.anchor0Y*r+this.control0Y*t,this.anchor0X*(r*r)+this.control0X*(2*r*t)+this.control1X*(t*t),this.anchor0Y*(r*r)+this.control0Y*(2*r*t)+this.control1Y*(t*t),i.x,i.y])),new at(new Float64Array([i.x,i.y,this.control0X*(r*r)+this.control1X*(2*r*t)+this.anchor1X*(t*t),this.control0Y*(r*r)+this.control1Y*(2*r*t)+this.anchor1Y*(t*t),this.control1X*r+this.anchor1X*t,this.control1Y*r+this.anchor1Y*t,this.anchor1X,this.anchor1Y]))]}reverse(){return new at(new Float64Array([this.anchor1X,this.anchor1Y,this.control1X,this.control1Y,this.control0X,this.control0Y,this.anchor0X,this.anchor0Y]))}transformed(t){let r=Float64Array.from(this.points);for(let i of[0,2,4,6]){let o=t(g(r[i],r[i+1]));r[i]=o.x,r[i+1]=o.y}return new at(r)}static empty(t,r){return new at(new Float64Array([t,r,t,r,t,r,t,r]))}static straightLine(t,r,i,o){return new at(new Float64Array([t,r,j(t,i,1/3),j(r,o,1/3),j(t,i,2/3),j(r,o,2/3),i,o]))}static circularArc(t,r,i,o,n,s){let l=At(i-t,o-r),a=At(n-t,s-r),c=_t(l),d=_t(a),h=yt(c,g(n-t,s-r))>=0,v=yt(l,a);if(v>.999)return at.straightLine(i,o,n,s);let u=Ft(i-t,o-r)*(4/3)*((Math.sqrt(2*(1-v))-Math.sqrt(1-v*v))/(1-v))*(h?1:-1);return new at(new Float64Array([i,o,i+c.x*u,o+c.y*u,n-d.x*u,s-d.y*u,n,s]))}};var Ue=class{constructor(e){if(e.length===0)throw new Error("Features need at least one cubic.");this.cubics=e}},Qt=class Hr extends Ue{transformed(t){return new Hr(this.cubics.map(r=>r.transformed(t)))}},q=class Ur extends Ue{constructor(t,r){super(t),this.convex=r}transformed(t){return new Ur(this.cubics.map(r=>r.transformed(t)),this.convex)}};var Ji=[[0,0],[.5,.5]],Ge=(e,t,r)=>{if(!(r>=0&&r<=1))throw new Error(`Invalid progress: ${r}`);let i=e.findIndex((a,c)=>He(r,a,e[(c+1)%e.length])),o=(i+1)%e.length,n=B(e[o]-e[i],1),s=B(t[o]-t[i],1),l=n<.001?.5:B(r-e[i],1)/n;return B(t[i]+s*l,1)},Wr=class{constructor(e){this.sourceValues=e.map(t=>t[0]),this.targetValues=e.map(t=>t[1]),Gr(this.sourceValues),Gr(this.targetValues)}map(e){return Ge(this.sourceValues,this.targetValues,e)}mapBack(e){return Ge(this.targetValues,this.sourceValues,e)}},Gr=e=>{let t=e[e.length-1],r=0;for(let i of e){if(!(i>=0&&i<1))throw new Error(`FloatMapping - Progress outside of range: ${e.join()}`);if(!(Dt(i,t)>1e-4))throw new Error(`FloatMapping - Progress repeats a value: ${e.join()}`);if(i<t&&(r++,r>1))throw new Error(`FloatMapping - Progress wraps more than once: ${e.join()}`);t=i}},Je=(e,t)=>{let r=e.filter(n=>n.feature instanceof q),i=t.filter(n=>n.feature instanceof q),o=Jr(r,i);return new Wr(o)},Jr=(e,t)=>{let r=[];for(let o of e)for(let n of t){let s=Qr(o.feature,n.feature);s!==Number.MAX_VALUE&&r.push({distance:s,f1:o,f2:n})}if(r.sort((o,n)=>o.distance-n.distance),r.length===0)return Ji;if(r.length===1){let{f1:o,f2:n}=r[0];return[[o.progress,n.progress],[(o.progress+.5)%1,(n.progress+.5)%1]]}let i=new Zi;for(let o of r)i.addMapping(o.f1,o.f2);return i.mapping},Qi=(e,t)=>{let r=0,i=e.length-1;for(;r<=i;){let o=r+i>>1,n=e[o][0]-t;if(n<0)r=o+1;else if(n>0)i=o-1;else return o}return-(r+1)},Zi=class{constructor(){this.mapping=[],this.usedF1=new Set,this.usedF2=new Set}addMapping(e,t){if(this.usedF1.has(e)||this.usedF2.has(t))return;let r=Qi(this.mapping,e.progress);if(r>=0)throw new Error("There can't be two features with the same progress");let i=-r-1,o=this.mapping.length;if(o>=1){let[n,s]=this.mapping[(i+o-1)%o],[l,a]=this.mapping[i%o];if(Dt(e.progress,n)<1e-4||Dt(e.progress,l)<1e-4||Dt(t.progress,s)<1e-4||Dt(t.progress,a)<1e-4||o>1&&!He(t.progress,s,a))return}this.mapping.splice(i,0,[e.progress,t.progress]),this.usedF1.add(e),this.usedF2.add(t)}},Qr=(e,t)=>e instanceof q&&t instanceof q&&e.convex!==t.convex?Number.MAX_VALUE:Br(We(e),We(t)),We=e=>{let t=e.cubics[0],r=e.cubics[e.cubics.length-1];return g((t.anchor0X+r.anchor1X)/2,(t.anchor0Y+r.anchor1Y)/2)};var ue=class de{static{this.Segments=3}measureCubic(t){return this.closestProgressTo(t,Number.POSITIVE_INFINITY)[1]}findCubicCutPoint(t,r){return this.closestProgressTo(t,r)[0]}closestProgressTo(t,r){let i=0,o=r,n=g(t.anchor0X,t.anchor0Y);for(let s=1;s<=de.Segments;s++){let l=s/de.Segments,a=t.pointOnCurve(l),c=gt(a,n);if(c>=o)return[l-(1-o/c)/de.Segments,r];o-=c,i+=c,n=a}return[1,i]}},Zr=class Qe{constructor(t,r,i,o){if(i<r)throw new Error("endOutlineProgress is expected to be equal or greater than startOutlineProgress");this.cubic=t,this.startOutlineProgress=r,this.endOutlineProgress=i,this.measurer=o,this.measuredSize=o.measureCubic(t)}updateProgressRange(t=this.startOutlineProgress,r=this.endOutlineProgress){if(r<t)throw new Error("endOutlineProgress is expected to be equal or greater than startOutlineProgress");this.startOutlineProgress=t,this.endOutlineProgress=r}cutAtProgress(t){let r=Math.min(this.endOutlineProgress,Math.max(this.startOutlineProgress,t)),i=this.endOutlineProgress-this.startOutlineProgress,o=(r-this.startOutlineProgress)/i,n=this.measurer.findCubicCutPoint(this.cubic,o*this.measuredSize);if(!(n>=0&&n<=1))throw new Error("Cubic cut point is expected to be between 0 and 1");let[s,l]=this.cubic.split(n);return[new Qe(s,this.startOutlineProgress,r,this.measurer),new Qe(l,r,this.endOutlineProgress,this.measurer)]}},pe=class Ze{constructor(t,r,i,o){if(o.length!==i.length+1)throw new Error("Outline progress size is expected to be the cubics size + 1");if(o[0]!==0)throw new Error("First outline progress value is expected to be zero");if(o[o.length-1]!==1)throw new Error("Last outline progress value is expected to be one");this.measurer=t,this.features=r;let n=[],s=0;for(let a=0;a<i.length;a++)o[a+1]-o[a]>1e-4&&(n.push(new Zr(i[a],s,o[a+1],t)),s=o[a+1]);let l=n[n.length-1];l.updateProgressRange(l.startOutlineProgress,1),this.measuredCubics=n}get size(){return this.measuredCubics.length}get(t){return this.measuredCubics[t]}getOrNull(t){return t>=0&&t<this.measuredCubics.length?this.measuredCubics[t]:null}cutAndShift(t){if(!(t>=0&&t<=1))throw new Error("Cutting point is expected to be between 0 and 1");if(t<1e-4)return this;let r=this.measuredCubics.findIndex(a=>t>=a.startOutlineProgress&&t<=a.endOutlineProgress),[i,o]=this.measuredCubics[r].cutAtProgress(t),n=[o.cubic];for(let a=1;a<this.measuredCubics.length;a++)n.push(this.measuredCubics[(a+r)%this.measuredCubics.length].cubic);n.push(i.cubic);let s=[];for(let a=0;a<this.measuredCubics.length+2;a++)if(a===0)s.push(0);else if(a===this.measuredCubics.length+1)s.push(1);else{let c=(r+a-1)%this.measuredCubics.length;s.push(B(this.measuredCubics[c].endOutlineProgress-t,1))}let l=this.features.map(a=>({progress:B(a.progress-t,1),feature:a.feature}));return new Ze(this.measurer,l,n,s)}static measure(t,r){let i=[],o=[];for(let c of r.features)for(let d=0;d<c.cubics.length;d++)c instanceof q&&d===Math.floor(c.cubics.length/2)&&o.push({feature:c,index:i.length}),i.push(c.cubics[d]);let n=[0],s=0;for(let c of i){let d=t.measureCubic(c);if(!(d>=0))throw new Error("Measured cubic is expected to be greater or equal to zero");s+=d,n.push(s)}let l=n.map(c=>c/s),a=o.map(({feature:c,index:d})=>({progress:B((l[d]+l[d+1])/2,1),feature:c}));return new Ze(t,a,i,l)}};var U={radius:0,smoothing:0},m=(e,t=0)=>({radius:e,smoothing:t}),Lt=class ti{constructor(t,r){this.features=t,this.designCenter=r,this.cubics=to(t,r);let i=this.cubics[this.cubics.length-1];for(let o of this.cubics){if(Math.abs(o.anchor0X-i.anchor1X)>1e-4||Math.abs(o.anchor0Y-i.anchor1Y)>1e-4)throw new Error("RoundedPolygon must be contiguous, with the anchor points of all curves matching the anchor points of the preceding and succeeding cubics");i=o}}get centerX(){return this.designCenter.x}get centerY(){return this.designCenter.y}get center(){return this.designCenter}transformed(t){return new ti(this.features.map(r=>r.transformed(t)),t(this.designCenter))}normalized(){let t=this.calculateBounds(new Float64Array(4)),r=t[2]-t[0],i=t[3]-t[1],o=Math.max(r,i);if(!(o>0))return this;let n=(o-r)/2-t[0],s=(o-i)/2-t[1];return this.transformed(l=>g((l.x+n)/o,(l.y+s)/o))}calculateMaxBounds(t=new Float64Array(4)){if(t.length<4)throw new Error("Required bounds size of 4");let r=0;for(let o of this.cubics){let n=ae(o.anchor0X-this.centerX,o.anchor0Y-this.centerY),s=o.pointOnCurve(.5),l=ae(s.x-this.centerX,s.y-this.centerY);r=Math.max(r,Math.max(n,l))}let i=Math.sqrt(r);return t[0]=this.centerX-i,t[1]=this.centerY-i,t[2]=this.centerX+i,t[3]=this.centerY+i,t}calculateBounds(t=new Float64Array(4),r=!0){if(t.length<4)throw new Error("Required bounds size of 4");let i=Number.MAX_VALUE,o=Number.MAX_VALUE,n=-Number.MAX_VALUE,s=-Number.MAX_VALUE,l=new Float64Array(4);for(let a of this.cubics)a.calculateBounds(l,r),i=Math.min(i,l[0]),o=Math.min(o,l[1]),n=Math.max(n,l[2]),s=Math.max(s,l[3]);return t[0]=i,t[1]=o,t[2]=n,t[3]=s,t}},to=(e,t)=>{let r=[],i=null,o=null,n=null,s=null;if(e.length>0&&e[0].cubics.length===3){let[l,a]=e[0].cubics[1].split(.5);n=[e[0].cubics[0],l],s=[a,e[0].cubics[2]]}for(let l=0;l<=e.length;l++){let a;if(l===0&&s!==null)a=s;else if(l===e.length)if(n!==null)a=n;else break;else a=e[l].cubics;for(let c of a)if(!c.zeroLength())o!==null&&r.push(o),o=c,i===null&&(i=c);else if(o!==null){let d=new z(Float64Array.from(o.points));d.points[6]=c.anchor1X,d.points[7]=c.anchor1Y,o=d}}return o!==null&&i!==null?r.push(new z(new Float64Array([o.anchor0X,o.anchor0Y,o.control0X,o.control0Y,o.control1X,o.control1Y,i.anchor0X,i.anchor0Y]))):r.push(z.empty(t.x,t.y)),r},tt=(e,t=U,r=null,i=null,o=null)=>{if(e.length<6)throw new Error("Polygons must have at least 3 vertices");if(e.length%2===1)throw new Error("The vertices array should have even size");if(r!==null&&r.length*2!==e.length)throw new Error("perVertexRounding list should be either null or the same size as the number of vertices (vertices.size / 2)");let n=e.length/2,s=[];for(let u=0;u<n;u++){let b=r!==null?r[u]:t,f=(u+n-1)%n*2,x=(u+1)%n*2;s.push(new oo(g(e[f],e[f+1]),g(e[u*2],e[u*2+1]),g(e[x],e[x+1]),b))}let l=[];for(let u=0;u<n;u++){let b=s[u].expectedRoundCut+s[(u+1)%n].expectedRoundCut,f=s[u].expectedCut+s[(u+1)%n].expectedCut,x=e[u*2],_=e[u*2+1],$=e[(u+1)%n*2],L=e[(u+1)%n*2+1],T=gt(g(x,_),g($,L));b>T?l.push([T/b,0]):f>T?l.push([1,(T-b)/(f-b)]):l.push([1,1])}let a=[];for(let u=0;u<n;u++){let b=[];for(let f=0;f<=1;f++){let[x,_]=l[(u+n-1+f)%n];b.push(s[u].expectedRoundCut*x+(s[u].expectedCut-s[u].expectedRoundCut)*_)}a.push(s[u].getCubics(b[0],b[1]))}let c=[];for(let u=0;u<n;u++){let b=(u+n-1)%n,f=(u+1)%n,x=g(e[u*2],e[u*2+1]),_=g(e[b*2],e[b*2+1]),$=g(e[f*2],e[f*2+1]),L=Jt(_,x,$);c.push(new q(a[u],L));let T=a[u][a[u].length-1],vt=a[(u+1)%n][0];c.push(new Qt([z.straightLine(T.anchor1X,T.anchor1Y,vt.anchor0X,vt.anchor0Y)]))}let d=io(e),h=i!==null&&o!==null?i:d[0],v=i!==null&&o!==null?o:d[1];return new Lt(c,g(h,v))},Mt=(e,t=1,r=0,i=0,o=U,n=null)=>{if(e<3)throw new Error("Polygons must have at least 3 vertices");let s=[];for(let l=0;l<e;l++){let a=D(ft(t,Math.PI/e*2*l),g(r,i));s.push(a.x,a.y)}return tt(s,o,n,r,i)},fe=(e=8,t=1,r=0,i=0)=>{if(e<3)throw new Error("Circle must have at least three vertices");let o=Math.PI/e,n=t/Math.cos(o);return Mt(e,n,r,i,m(t))},Vt=(e=2,t=2,r=U,i=null,o=0,n=0)=>{if(!(e>0&&t>0))throw new Error("Rectangle shapes must have positive width and height");let s=o-e/2,l=n-t/2,a=o+e/2,c=n+t/2;return tt([a,c,s,c,s,l,a,l],r,i,o,n)},mt=(e,t=1,r=.5,i=U,o=null,n=null,s=0,l=0)=>{if(t<=0||r<=0)throw new Error("Star radii must both be greater than 0");if(r>=t)throw new Error("innerRadius must be less than radius");if(e<2)throw new Error("Star must have at least 2 vertices per radius");let a=n;if(a===null&&o!==null){let d=[];for(let h=0;h<e;h++)d.push(i,o);a=d}let c=[];for(let d=0;d<e;d++){let h=ft(t,Math.PI/e*2*d);c.push(h.x+s,h.y+l);let v=ft(r,Math.PI/e*(2*d+1));c.push(v.x+s,v.y+l)}return tt(c,i,a,s,l)},eo=(e=2,t=1,r=0,i=0,o=0)=>{if(!(e>0&&t>0))throw new Error("Pill shapes must have positive width and height");let n=e/2,s=t/2;return tt([n+i,s+o,-n+i,s+o,-n+i,-s+o,n+i,-s+o],m(Math.min(n,s),r),null,i,o)},tr=(e=2,t=1,r=8,i=.5,o=U,n=null,s=null,l=.5,a=0,c=0,d=0)=>{if(!(e>0&&t>0))throw new Error("Pill shapes must have positive width and height");if(!(i>0&&i<=1))throw new Error("innerRadius must be between 0 and 1");if(r<2)throw new Error("pillStar must have at least 2 vertices per radius");let h=s;if(h===null&&n!==null){let u=[];for(let b=0;b<r;b++)u.push(o,n);h=u}let v=ro(r,e,t,i,l,a,c,d);return tt(v,o,h,c,d)},ro=(e,t,r,i,o,n,s,l)=>{let a=Math.min(t,r),c=Math.max(0,r-t),d=Math.max(0,t-r),h=c/2,v=d/2,u=2*Math.PI*a*j(i,1,o),b=2*d+2*c+u,f=new Float64Array(11);f[0]=0,f[1]=c/2,f[2]=f[1]+u/4,f[3]=f[2]+d,f[4]=f[3]+u/4,f[5]=f[4]+c,f[6]=f[5]+u/4,f[7]=f[6]+d,f[8]=f[7]+u/4,f[9]=f[8]+c/2,f[10]=b;let x=b/(2*e),_=!1,$=0,L=0,T=f[1],vt=n*b,X=[],ct=g(v,h),F=g(-v,h),O=g(-v,-h),J=g(v,-h);for(let ht=0;ht<e*2;ht++){let et=vt%b;for(et<L&&($=0);et>=f[($+1)%f.length];)$=($+1)%f.length,L=f[$],T=f[($+1)%f.length];let xt=et-L,rt=T>L?xt/(T-L):0,it=_?a*i:a,N;switch($){case 0:N=g(it,rt*h);break;case 1:N=D(ft(it,rt*Math.PI/2),ct);break;case 2:N=g(v-rt*d,it);break;case 3:N=D(ft(it,Math.PI/2+rt*Math.PI/2),F);break;case 4:N=g(-it,h-rt*c);break;case 5:N=D(ft(it,Math.PI+rt*Math.PI/2),O);break;case 6:N=g(-v+rt*d,-it);break;case 7:N=D(ft(it,Math.PI*1.5+rt*Math.PI/2),J);break;default:N=g(it,-h+rt*h)}X.push(N.x+s,N.y+l),vt+=x,_=!_}return X},io=e=>{let t=0,r=0,i=0;for(;i<e.length;)t+=e[i++],r+=e[i++];return[t/(e.length/2),r/(e.length/2)]},oo=class{constructor(e,t,r,i){this.center=g(0,0),this.p0=e,this.p1=t,this.p2=r;let o=Z(e,t),n=Z(r,t),s=gt(e,t),l=gt(r,t);s>0&&l>0?(this.d1=K(o,1/s),this.d2=K(n,1/l),this.cornerRadius=i.radius,this.smoothing=i.smoothing,this.cosAngle=yt(this.d1,this.d2),this.sinAngle=Math.sqrt(Math.max(0,1-le(this.cosAngle))),this.expectedRoundCut=this.sinAngle>.001?this.cornerRadius*(this.cosAngle+1)/this.sinAngle:0):(this.d1=g(0,0),this.d2=g(0,0),this.cornerRadius=0,this.smoothing=0,this.cosAngle=0,this.sinAngle=0,this.expectedRoundCut=0)}get expectedCut(){return(1+this.smoothing)*this.expectedRoundCut}getCubics(e,t=e){let r=Math.min(e,t);if(this.expectedRoundCut<1e-4||r<1e-4||this.cornerRadius<1e-4)return this.center=this.p1,[z.straightLine(this.p1.x,this.p1.y,this.p1.x,this.p1.y)];let i=Math.min(r,this.expectedRoundCut),o=this.calculateActualSmoothingValue(e),n=this.calculateActualSmoothingValue(t),s=this.cornerRadius*i/this.expectedRoundCut,l=Math.sqrt(le(s)+le(i)),a=g((this.d1.x+this.d2.x)/2,(this.d1.y+this.d2.y)/2);this.center=D(this.p1,K(At(a.x,a.y),l));let c=D(this.p1,K(this.d1,i)),d=D(this.p1,K(this.d2,i)),h=this.computeFlankingCurve(i,o,this.p1,this.p0,c,d,this.center,s),v=this.computeFlankingCurve(i,n,this.p1,this.p2,d,c,this.center,s).reverse();return[h,z.circularArc(this.center.x,this.center.y,h.anchor1X,h.anchor1Y,v.anchor0X,v.anchor0Y),v]}calculateActualSmoothingValue(e){return e>this.expectedCut?this.smoothing:e>this.expectedRoundCut?this.smoothing*(e-this.expectedRoundCut)/(this.expectedCut-this.expectedRoundCut):0}computeFlankingCurve(e,t,r,i,o,n,s,l){let a=At(i.x-r.x,i.y-r.y),c=D(r,K(a,e*(1+t))),d=Xr(o,g((o.x+n.x)/2,(o.y+n.y)/2),t),h=D(s,K(At(d.x-s.x,d.y-s.y),l)),v=_t(Z(h,s)),u=this.lineIntersection(i,a,h,v)??o,b=K(D(c,K(u,2)),1/3);return new z(new Float64Array([c.x,c.y,b.x,b.y,u.x,u.y,h.x,h.y]))}lineIntersection(e,t,r,i){let o=_t(i),n=yt(t,o);if(Math.abs(n)<1e-4)return null;let s=yt(Z(r,e),o);if(Math.abs(n)<1e-4*Math.abs(s))return null;let l=s/n;return D(e,K(t,l))}};var Zt=class{constructor(e,t){this.start=e,this.end=t,this.morphMatch=no(e,t),this.startCenter=er(e.cubics),this.endCenter=er(t.cubics)}get curveCount(){return this.morphMatch.length}calculateBounds(e=new Float64Array(4),t=!0){if(e.length<4)throw new Error("Required bounds size of 4");this.start.calculateBounds(e,t);let r=e[0],i=e[1],o=e[2],n=e[3];return this.end.calculateBounds(e,t),e[0]=Math.min(r,e[0]),e[1]=Math.min(i,e[1]),e[2]=Math.max(o,e[2]),e[3]=Math.max(n,e[3]),e}calculateMaxBounds(e=new Float64Array(4)){if(e.length<4)throw new Error("Required bounds size of 4");this.start.calculateMaxBounds(e);let t=e[0],r=e[1],i=e[2],o=e[3];return this.end.calculateMaxBounds(e),e[0]=Math.min(t,e[0]),e[1]=Math.min(r,e[1]),e[2]=Math.max(i,e[2]),e[3]=Math.max(o,e[3]),e}centerAt(e){return g(j(this.startCenter.x,this.endCenter.x,e),j(this.startCenter.y,this.endCenter.y,e))}asCubics(e){let t=[],r=null,i=null;for(let[o,n]of this.morphMatch){let s=new Float64Array(8);for(let a=0;a<8;a++)s[a]=j(o.points[a],n.points[a],e);let l=new z(s);r===null&&(r=l),i!==null&&t.push(i),i=l}return i!==null&&r!==null&&t.push(new z(new Float64Array([i.anchor0X,i.anchor0Y,i.control0X,i.control0Y,i.control1X,i.control1Y,r.anchor0X,r.anchor0Y]))),t}toPolygon(e){if(e===0)return this.start;if(e===1)return this.end;let t=this.asCubics(e).filter(o=>!o.zeroLength());if(t.length===0)return e<.5?this.start:this.end;let r=t.map((o,n)=>{let s=t[(n+1)%t.length];return o.anchor1X===s.anchor0X&&o.anchor1Y===s.anchor0Y?o:new z(new Float64Array([o.anchor0X,o.anchor0Y,o.control0X,o.control0Y,o.control1X,o.control1Y,s.anchor0X,s.anchor0Y]))}),i=[];for(let o=0;o<r.length;o++){let n=r[(o+r.length-1)%r.length],s=r[o];if(he(s.anchor0X,s.anchor0Y,s.control0X,s.control0Y,s.anchor1X,s.anchor1Y,.005)&&he(s.anchor0X,s.anchor0Y,s.control1X,s.control1Y,s.anchor1X,s.anchor1Y,.005)){if(!he(n.anchor0X,n.anchor0Y,s.anchor0X,s.anchor0Y,s.anchor1X,s.anchor1Y,.005)){let l=Jt(g(n.anchor0X,n.anchor0Y),g(s.anchor0X,s.anchor0Y),g(s.anchor1X,s.anchor1Y));i.push(new q([z.empty(s.anchor0X,s.anchor0Y)],l))}i.push(new Qt([s]))}else{let l=s.pointOnCurve(.5),a=Jt(g(s.anchor0X,s.anchor0Y),l,g(s.anchor1X,s.anchor1Y));i.push(new q([s],a))}}return new Lt(i,this.centerAt(e))}},no=(e,t)=>{let r=pe.measure(new ue,e),i=pe.measure(new ue,t),o=Je(r.features,i.features),n=o.map(0),s=r,l=i.cutAndShift(n),a=[],c=0,d=0,h=s.getOrNull(c++),v=l.getOrNull(d++);for(;h!==null&&v!==null;){let u=c===s.size?1:h.endOutlineProgress,b=d===l.size?1:o.mapBack(B(v.endOutlineProgress+n,1)),f=Math.min(u,b),x,_;if(u>f+1e-6){let T=h.cutAtProgress(f);x=T[0],_=T[1]}else x=h,_=s.getOrNull(c++);let $,L;if(b>f+1e-6){let T=v.cutAtProgress(B(o.map(f)-n,1));$=T[0],L=T[1]}else $=v,L=l.getOrNull(d++);a.push([x.cubic,$.cubic]),h=_,v=L}if(h!==null||v!==null)throw new Error("Expected both Polygon's Cubic to be fully matched");return a},rr=(e,t={})=>{if(e.length===0)return"";let{rotation:r=0,startAngle:i=0,rotationPivotX:o=.5,rotationPivotY:n=.5,scaleX:s=1,scaleY:l=1,centerX:a,centerY:c,sourceCenterX:d,sourceCenterY:h,repeatPath:v=!1,closePath:u=!0,precision:b=4}=t,f=F=>F.toFixed(b),x=r;if(i!==0){let F=Math.atan2(e[0].anchor0Y-n,e[0].anchor0X-o)*180/Math.PI;x+=-F+i}let _=d??o,$=h??n,L=a??_*s,T=c??$*l,vt=g(_,$),X=(F,O)=>{let J=x!==0?$t(g(F,O),x,vt):g(F,O);return g((J.x-_)*s+L,(J.y-$)*l+T)},ct="";for(let F=0;F<e.length;F++){let O=e[F];if(F===0){let xt=X(O.anchor0X,O.anchor0Y);ct+=`M${f(xt.x)} ${f(xt.y)}`}let J=X(O.control0X,O.control0Y),ht=X(O.control1X,O.control1Y),et=X(O.anchor1X,O.anchor1Y);ct+=`C${f(J.x)} ${f(J.y)} ${f(ht.x)} ${f(ht.y)} ${f(et.x)} ${f(et.y)}`}if(v)for(let F=0;F<e.length;F++){let O=e[F];if(F===0){let xt=X(O.anchor0X,O.anchor0Y);ct+=`L${f(xt.x)} ${f(xt.y)}`}let J=X(O.control0X,O.control0Y),ht=X(O.control1X,O.control1Y),et=X(O.anchor1X,O.anchor1Y);ct+=`C${f(J.x)} ${f(J.y)} ${f(ht.x)} ${f(ht.y)} ${f(et.x)} ${f(et.y)}`}return u?`${ct}Z`:ct},ei=e=>{if(e.length===0)throw new Error("Cannot compute bounds of an empty cubic list");let t=Number.MAX_VALUE,r=Number.MAX_VALUE,i=-Number.MAX_VALUE,o=-Number.MAX_VALUE;for(let n of e)t=Math.min(t,n.anchor0X,n.control0X,n.control1X,n.anchor1X),r=Math.min(r,n.anchor0Y,n.control0Y,n.control1Y,n.anchor1Y),i=Math.max(i,n.anchor0X,n.control0X,n.control1X,n.anchor1X),o=Math.max(o,n.anchor0Y,n.control0Y,n.control1Y,n.anchor1Y);return g((t+i)/2,(r+o)/2)},er=(e,t=8)=>{if(e.length===0)throw new Error("Cannot compute centroid of an empty cubic list");let r=0,i=0,o=0;for(let s of e){let l=s.anchor0X,a=s.anchor0Y;for(let c=1;c<=t;c++){let d=s.pointOnCurve(c/t),h=l*d.y-d.x*a;r+=h,i+=(l+d.x)*h,o+=(a+d.y)*h,l=d.x,a=d.y}}if(Math.abs(r)<1e-12)return ei(e);let n=1/(3*r);return g(i*n,o*n)};var so=m(.15),te=m(.2),ao=m(.3),ir=m(.5),ye=m(1),lo=e=>$t(e,-45),ge=e=>$t(e,-90),co=e=>$t(e,-135),p=(e,t,r=U)=>({o:g(e,t),r}),ho=(e,t,r,i)=>{if(i){let s=e.map(h=>Nr(Z(h.o,r))),l=e.map(h=>gt(h.o,r)),a=t*2,c=360/a,d=[];for(let h=0;h<a;h++)for(let v=0;v<e.length;v++){let u=h%2===0?v:e.length-1-v;if(u>0||h%2===0){let b=c*h+(h%2===0?s[u]:c-s[u]+2*s[0]),f=D(K(g(Math.cos(ce(b)),Math.sin(ce(b))),l[u]),r);d.push({o:f,r:e[u].r})}}return d}let o=[],n=e.length;for(let s=0;s<n*t;s++){let l=e[s%n];o.push({o:$t(l.o,Math.floor(s/n)*360/t,r),r:l.r})}return o},k=(e,t,r=g(.5,.5),i=!1)=>{let o=ho(e,t,r,i),n=[],s=[];for(let l of o)n.push(l.o.x,l.o.y),s.push(l.r);return tt(n,U,s,r.x,r.y)},ri=new Map,A=(e,t)=>{let r=ri.get(e);return r||(r=t().normalized(),ri.set(e,r)),r},P={get Circle(){return A(y.Circle,()=>fe(10))},get Square(){return A(y.Square,()=>Vt(1,1,ao))},get Slanted(){return A(y.Slanted,()=>k([p(.926,.97,m(.189,.811)),p(-.021,.967,m(.187,.057))],2))},get Arch(){return A(y.Arch,()=>Mt(4,1,0,0,U,[ye,ye,te,te]).transformed(co))},get Fan(){return A(y.Fan,()=>k([p(1.004,1,m(.148,.417)),p(0,1,m(.151)),p(0,-.003,m(.148)),p(.978,.02,m(.803))],1))},get Arrow(){return A(y.Arrow,()=>k([p(.5,.892,m(.313)),p(-.216,1.05,m(.207)),p(.499,-.16,m(.215,1)),p(1.225,1.06,m(.211))],1))},get SemiCircle(){return A(y.SemiCircle,()=>Vt(1.6,1,U,[te,te,ye,ye]))},get Oval(){return A(y.Oval,()=>fe().transformed(e=>g(e.x,e.y*.64)).transformed(lo))},get Pill(){return A(y.Pill,()=>k([p(.961,.039,m(.426)),p(1.001,.428),p(1,.609,m(1))],2,g(.5,.5),!0))},get Triangle(){return A(y.Triangle,()=>Mt(3,1,0,0,te).transformed(ge))},get Diamond(){return A(y.Diamond,()=>k([p(.5,1.096,m(.151,.524)),p(.04,.5,m(.159))],2))},get ClamShell(){return A(y.ClamShell,()=>k([p(.171,.841,m(.159)),p(-.02,.5,m(.14)),p(.17,.159,m(.159))],2))},get Pentagon(){return A(y.Pentagon,()=>k([p(.5,-.009,m(.172)),p(1.03,.365,m(.164)),p(.828,.97,m(.169))],1,g(.5,.5),!0))},get Gem(){return A(y.Gem,()=>k([p(.499,1.023,m(.241,.778)),p(-.005,.792,m(.208)),p(.073,.258,m(.228)),p(.433,-0,m(.491))],1,g(.5,.5),!0))},get Sunny(){return A(y.Sunny,()=>mt(8,1,.8,so))},get VerySunny(){return A(y.VerySunny,()=>k([p(.5,1.08,m(.085)),p(.358,.843,m(.085))],8))},get Cookie4Sided(){return A(y.Cookie4Sided,()=>k([p(1.237,1.236,m(.258)),p(.5,.918,m(.233))],4))},get Cookie6Sided(){return A(y.Cookie6Sided,()=>k([p(.723,.884,m(.394)),p(.5,1.099,m(.398))],6))},get Cookie7Sided(){return A(y.Cookie7Sided,()=>mt(7,1,.75,ir).transformed(ge))},get Cookie9Sided(){return A(y.Cookie9Sided,()=>mt(9,1,.8,ir).transformed(ge))},get Cookie12Sided(){return A(y.Cookie12Sided,()=>mt(12,1,.8,ir).transformed(ge))},get Ghostish(){return A(y.Ghostish,()=>k([p(.5,0,m(1)),p(1,0,m(1)),p(1,1.14,m(.254,.106)),p(.575,.906,m(.253))],1,g(.5,.5),!0))},get Clover4Leaf(){return A(y.Clover4Leaf,()=>k([p(.5,.074),p(.725,-.099,m(.476))],4,g(.5,.5),!0))},get Clover8Leaf(){return A(y.Clover8Leaf,()=>k([p(.5,.036),p(.758,-.101,m(.209))],8))},get Burst(){return A(y.Burst,()=>k([p(.5,-.006,m(.006)),p(.592,.158,m(.006))],12))},get SoftBurst(){return A(y.SoftBurst,()=>k([p(.193,.277,m(.053)),p(.176,.055,m(.053))],10))},get Boom(){return A(y.Boom,()=>k([p(.457,.296,m(.007)),p(.5,-.051,m(.007))],15))},get SoftBoom(){return A(y.SoftBoom,()=>k([p(.733,.454),p(.839,.437,m(.532)),p(.949,.449,m(.439,1)),p(.998,.478,m(.174))],16,g(.5,.5),!0))},get Flower(){return A(y.Flower,()=>k([p(.37,.187),p(.416,.049,m(.381)),p(.479,.001,m(.095))],8,g(.5,.5),!0))},get Puffy(){return A(y.Puffy,()=>k([p(.5,.053),p(.545,-.04,m(.405)),p(.67,-.035,m(.426)),p(.717,.066,m(.574)),p(.722,.128),p(.777,.002,m(.36)),p(.914,.149,m(.66)),p(.926,.289,m(.66)),p(.881,.346),p(.94,.344,m(.126)),p(1.003,.437,m(.255))],2,g(.5,.5),!0).transformed(e=>g(e.x,e.y*.742)))},get PuffyDiamond(){return A(y.PuffyDiamond,()=>k([p(.87,.13,m(.146)),p(.818,.357),p(1,.332,m(.853))],4,g(.5,.5),!0))},get PixelCircle(){return A(y.PixelCircle,()=>k([p(.5,0),p(.704,0),p(.704,.065),p(.843,.065),p(.843,.148),p(.926,.148),p(.926,.296),p(1,.296)],2,g(.5,.5),!0))},get PixelTriangle(){return A(y.PixelTriangle,()=>k([p(.11,.5),p(.113,0),p(.287,0),p(.287,.087),p(.421,.087),p(.421,.17),p(.56,.17),p(.56,.265),p(.674,.265),p(.675,.344),p(.789,.344),p(.789,.439),p(.888,.439)],1,g(.5,.5),!0))},get Bun(){return A(y.Bun,()=>k([p(.796,.5),p(.853,.518,m(1)),p(.992,.631,m(1)),p(.968,1,m(1))],2,g(.5,.5),!0))},get Heart(){return A(y.Heart,()=>k([p(.5,.268,m(.016)),p(.792,-.066,m(.958)),p(1.064,.276,m(1)),p(.501,.946,m(.129))],1,g(.5,.5),!0))}},uo={[y.Circle]:()=>P.Circle,[y.Square]:()=>P.Square,[y.Slanted]:()=>P.Slanted,[y.Arch]:()=>P.Arch,[y.Fan]:()=>P.Fan,[y.Arrow]:()=>P.Arrow,[y.SemiCircle]:()=>P.SemiCircle,[y.Oval]:()=>P.Oval,[y.Pill]:()=>P.Pill,[y.Triangle]:()=>P.Triangle,[y.Diamond]:()=>P.Diamond,[y.ClamShell]:()=>P.ClamShell,[y.Pentagon]:()=>P.Pentagon,[y.Gem]:()=>P.Gem,[y.Sunny]:()=>P.Sunny,[y.VerySunny]:()=>P.VerySunny,[y.Cookie4Sided]:()=>P.Cookie4Sided,[y.Cookie6Sided]:()=>P.Cookie6Sided,[y.Cookie7Sided]:()=>P.Cookie7Sided,[y.Cookie9Sided]:()=>P.Cookie9Sided,[y.Cookie12Sided]:()=>P.Cookie12Sided,[y.Ghostish]:()=>P.Ghostish,[y.Clover4Leaf]:()=>P.Clover4Leaf,[y.Clover8Leaf]:()=>P.Clover8Leaf,[y.Burst]:()=>P.Burst,[y.SoftBurst]:()=>P.SoftBurst,[y.Boom]:()=>P.Boom,[y.SoftBoom]:()=>P.SoftBoom,[y.Flower]:()=>P.Flower,[y.Puffy]:()=>P.Puffy,[y.PuffyDiamond]:()=>P.PuffyDiamond,[y.PixelCircle]:()=>P.PixelCircle,[y.PixelTriangle]:()=>P.PixelTriangle,[y.Bun]:()=>P.Bun,[y.Heart]:()=>P.Heart},or=Object.values(y),nr=e=>{let t=uo[e];if(!t)throw new Error(`Unknown MaterialShape: "${String(e)}"`);return t()};var sr={Expressive:{FastSpatial:{stiffness:800,dampingRatio:.6},DefaultSpatial:{stiffness:380,dampingRatio:.8},SlowSpatial:{stiffness:200,dampingRatio:.8},FastEffects:{stiffness:3800,dampingRatio:1},DefaultEffects:{stiffness:1600,dampingRatio:1},SlowEffects:{stiffness:800,dampingRatio:1}},Standard:{FastSpatial:{stiffness:1400,dampingRatio:.9},DefaultSpatial:{stiffness:700,dampingRatio:.9},SlowSpatial:{stiffness:300,dampingRatio:.9},FastEffects:{stiffness:3800,dampingRatio:1},DefaultEffects:{stiffness:1600,dampingRatio:1},SlowEffects:{stiffness:800,dampingRatio:1}}},ar=(e,t)=>{let r=e==="standard"?sr.Standard:sr.Expressive;switch(t){case"fast":return r.FastSpatial;case"slow":return r.SlowSpatial;case"default":return r.DefaultSpatial}},po=Number.MAX_SAFE_INTEGER,ii=({stiffness:e,dampingRatio:t})=>{if(!Number.isFinite(e)||!(e>0))throw new Error("Spring stiffness constant must be positive.");if(!Number.isFinite(t)||!(t>=0))throw new Error("Damping ratio must be non-negative")},me=e=>(t,r,i,o)=>{ii(e);let n=Math.sqrt(e.stiffness),{dampingRatio:s}=e,l=t-o,a=Math.max(0,i)/1e3,c=s*s,d=-s*n,h,v;if(s>1){let u=n*Math.sqrt(c-1),b=d+u,f=d-u,x=(f*l-r)/(f-b),_=l-x;h=_*Math.exp(f*a)+x*Math.exp(b*a),v=_*f*Math.exp(f*a)+x*b*Math.exp(b*a)}else if(s===1){let u=l,b=r+n*l,f=-n*a;h=(u+b*a)*Math.exp(f),v=(u+b*a)*Math.exp(f)*-n+b*Math.exp(f)}else{let u=n*Math.sqrt(1-c),b=l,f=1/u*(-d*l+r),x=u*a;h=Math.exp(d*a)*(b*Math.cos(x)+f*Math.sin(x)),v=h*d+Math.exp(d*a)*(-u*b*Math.sin(x)+u*f*Math.cos(x))}return{value:h+o,velocity:v}},fo=e=>{let{stiffness:t,dampingRatio:r,initialVelocity:i,initialDisplacement:o,delta:n}=e;if(ii({stiffness:t,dampingRatio:r}),r===0)return po;let s=2*r*Math.sqrt(t),l=s*s-4*t,a=l<0?0:Math.sqrt(l),c=l<0?Math.sqrt(Math.abs(l)):0,d=(-s+a)*.5,h=c*.5,v=(-s-a)*.5;return yo(d,h,v,r,i,o,n)},yo=(e,t,r,i,o,n,s)=>{if(n===0&&o===0)return 0;let l=n<0?-o:o,a=Math.abs(n),c=i>1?bo(e,r,a,l,s):i<1?go(e,t,a,l,s):mo(e,a,l,s);return Math.trunc(c*1e3)},oi=(e,t,r)=>e-t(e)/r(e),go=(e,t,r,i,o)=>{let n=e,s=r,l=(i-n*s)/t,a=Math.sqrt(s*s+l*l);return Math.log(o/a)/n},mo=(e,t,r,i)=>{let o=e,n=t,s=r-o*n,l=Math.log(Math.abs(i/n))/o,a=(()=>{let f=Math.log(Math.abs(i/s)),x=f;for(let _=0;_<=5;_++)x=f-Math.log(Math.abs(x/o));return x})()/o,c=Number.isFinite(l)?Number.isFinite(a)?Math.max(l,a):l:a,d=-(o*n+s)/(o*s),h=n*Math.exp(o*d)+s*d*Math.exp(o*d),v;Number.isNaN(d)||d<=0?v=-i:-h<i?(s<0&&n>0&&(c=0),v=-i):(c=-(2/o)-n/s,v=i);let u=Number.MAX_VALUE,b=0;for(;u>.001&&b<100;){b++;let f=c;c=oi(c,x=>(n+s*x)*Math.exp(o*x)+v,x=>(s*(o*x+1)+n*o)*Math.exp(o*x)),u=Math.abs(f-c)}return c},bo=(e,t,r,i,o)=>{let n=e,s=t,l=(n*r-i)/(n-s),a=r-l,c=Math.log(Math.abs(o/a))/n,d=Math.log(Math.abs(o/l))/s,h=Number.isFinite(c)?Number.isFinite(d)?Math.max(c,d):c:d,v=Math.log(a*n/(-l*s))/(s-n),u=()=>a*Math.exp(n*v)+l*Math.exp(s*v),b;if(Number.isNaN(v)||v<=0?b=-o:-u()<o?(l>0&&a<0&&(h=0),b=-o):(h=Math.log(-(l*s*s)/(a*n*n))/(n-s),b=o),Math.abs(a*n*Math.exp(n*h)+l*s*Math.exp(s*h))<1e-4)return h;let f=Number.MAX_VALUE,x=0;for(;f>.001&&x<100;){x++;let _=h;h=oi(h,$=>a*Math.exp(n*$)+l*Math.exp(s*$)+b,$=>a*n*Math.exp(n*$)+l*s*Math.exp(s*$)),f=Math.abs(_-h)}return h};var So=["ariaAtomic","ariaAutoComplete","ariaBrailleLabel","ariaBrailleRoleDescription","ariaBusy","ariaChecked","ariaColCount","ariaColIndex","ariaColIndexText","ariaColSpan","ariaCurrent","ariaDescription","ariaDisabled","ariaExpanded","ariaHasPopup","ariaHidden","ariaInvalid","ariaKeyShortcuts","ariaLabel","ariaLevel","ariaLive","ariaModal","ariaMultiLine","ariaMultiSelectable","ariaOrientation","ariaPlaceholder","ariaPosInSet","ariaPressed","ariaReadOnly","ariaRelevant","ariaRequired","ariaRoleDescription","ariaRowCount","ariaRowIndex","ariaRowIndexText","ariaRowSpan","ariaSelected","ariaSetSize","ariaSort","ariaValueMax","ariaValueMin","ariaValueNow","ariaValueText","role"],lr=new Set(So),vo=new Set(lr.values().map(cr));function be(e){return vo.has(e)}function cr(e){return e.replace("aria","aria-").replace(/Elements?/g,"").toLowerCase()}var Se=Symbol("privateIgnoreAttributeChangesFor");function ni(e){if(!1)return e;class t extends e{constructor(...i){super(...i),this[Se]=new Set}attributeChangedCallback(i,o,n){if(!be(i)){super.attributeChangedCallback(i,o,n);return}if(this[Se].has(i))return;this[Se].add(i),this.removeAttribute(i),this[Se].delete(i);let s=dr(i);n===null?delete this.dataset[s]:this.dataset[s]=n,this.requestUpdate(dr(i),o)}getAttribute(i){return be(i)?super.getAttribute(hr(i)):super.getAttribute(i)}removeAttribute(i){super.removeAttribute(i),be(i)&&(super.removeAttribute(hr(i)),this.requestUpdate())}}return xo(t),t}function xo(e){for(let t of lr){let r=cr(t),i=hr(r),o=dr(r);e.createProperty(t,{attribute:r,noAccessor:!0}),e.createProperty(Symbol(i),{attribute:i,noAccessor:!0}),Object.defineProperty(e.prototype,t,{configurable:!0,enumerable:!0,get(){return this.dataset[o]??null},set(n){let s=this.dataset[o]??null;n!==s&&(n===null?delete this.dataset[o]:this.dataset[o]=n,this.requestUpdate(t,s))}})}}function hr(e){return`data-${e}`}function dr(e){return e.replace(/-\w/,t=>t[1].toUpperCase())}function si(...e){return function(t){return e.reduce((r,i)=>i(r),t)}}function S(e,t,r,i){var o=arguments.length,n=o<3?t:i===null?i=Object.getOwnPropertyDescriptor(t,r):i,s;if(typeof Reflect=="object"&&typeof Reflect.decorate=="function")n=Reflect.decorate(e,t,r,i);else for(var l=e.length-1;l>=0;l--)(s=e[l])&&(n=(o<3?s(n):o>3?s(t,r,n):s(t,r))||n);return o>3&&n&&Object.defineProperty(t,r,n),n}var bt=class extends Event{constructor(t,r,i,o){super("context-request",{bubbles:!0,composed:!0}),this.context=t,this.contextTarget=r,this.callback=i,this.subscribe=o??!1}};var zt=class{constructor(t,r,i,o){if(this.subscribe=!1,this.provided=!1,this.value=void 0,this.t=(n,s)=>{this.unsubscribe&&(this.unsubscribe!==s&&(this.provided=!1,this.unsubscribe()),this.subscribe||this.unsubscribe()),this.value=n,this.host.requestUpdate(),this.provided&&!this.subscribe||(this.provided=!0,this.callback&&this.callback(n,s)),this.unsubscribe=s},this.host=t,r.context!==void 0){let n=r;this.context=n.context,this.callback=n.callback,this.subscribe=n.subscribe??!1}else this.context=r,this.callback=i,this.subscribe=o??!1;this.host.addController(this)}hostConnected(){this.dispatchRequest()}hostDisconnected(){this.unsubscribe&&(this.unsubscribe(),this.unsubscribe=void 0)}dispatchRequest(){this.host.dispatchEvent(new bt(this.context,this.host,this.t,this.subscribe))}};var ve=class{get value(){return this.o}set value(t){this.setValue(t)}setValue(t,r=!1){let i=r||!Object.is(t,this.o);this.o=t,i&&this.updateObservers()}constructor(t){this.subscriptions=new Map,this.updateObservers=()=>{for(let[r,{disposer:i}]of this.subscriptions)r(this.o,i)},t!==void 0&&(this.value=t)}addCallback(t,r,i){if(!i)return void t(this.value);this.subscriptions.has(t)||this.subscriptions.set(t,{disposer:()=>{this.subscriptions.delete(t)},consumerHost:r});let{disposer:o}=this.subscriptions.get(t);t(this.value,o)}clearCallbacks(){this.subscriptions.clear()}};var ur=class extends Event{constructor(t,r){super("context-provider",{bubbles:!0,composed:!0}),this.context=t,this.contextTarget=r}},It=class extends ve{constructor(t,r,i){super(r.context!==void 0?r.initialValue:i),this.onContextRequest=o=>{if(o.context!==this.context)return;let n=o.contextTarget??o.composedPath()[0];n!==this.host&&(o.stopPropagation(),this.addCallback(o.callback,n,o.subscribe))},this.onProviderRequest=o=>{if(o.context!==this.context||(o.contextTarget??o.composedPath()[0])===this.host)return;let n=new Set;for(let[s,{consumerHost:l}]of this.subscriptions)n.has(s)||(n.add(s),l.dispatchEvent(new bt(this.context,l,s,!0)));o.stopPropagation()},this.host=t,r.context!==void 0?this.context=r.context:this.context=r,this.attachListeners(),this.host.addController?.(this)}attachListeners(){this.host.addEventListener("context-request",this.onContextRequest),this.host.addEventListener("context-provider",this.onProviderRequest)}hostConnected(){this.host.dispatchEvent(new ur(this.context,this.host))}};var xe=Symbol.for("mdc-global-context"),wo=class ai{constructor(){this.provider=null,this.currentConfig={ripple:{},focusRing:{},elevation:{}}}static{this._Instance=null}static get Instance(){return this._Instance||(this._Instance=new ai),this._Instance}mergeConfig(t){return{...this.currentConfig,...t,ripple:{...this.currentConfig.ripple,...t.ripple??{}},focusRing:{...this.currentConfig.focusRing,...t.focusRing??{}},elevation:{...this.currentConfig.elevation,...t.elevation??{}}}}attach(t={}){this.provider||(this.currentConfig=this.mergeConfig(t),this.provider=new It(document.documentElement,{initialValue:this.currentConfig,context:xe}))}setConfig(t){if(!this.provider)throw new Error("GlobalMDCContextProvider is not attached yet. Call attach() first.");this.currentConfig=this.mergeConfig(t),this.provider.setValue(this.currentConfig)}},Eo=wo.Instance;var Po=/[\t\n\f\r ]/,li=e=>{if(e!==""&&Po.test(e))throw new Error(`Invalid "for" id ${JSON.stringify(e)}: HTML IDs must not contain ASCII whitespace`)},pr=class{get htmlFor(){return this.host.getAttribute("for")}set htmlFor(e){e===null?this.host.removeAttribute("for"):(li(e),this.host.setAttribute("for",e))}get control(){if(this.host.hasAttribute("for")){let e=this.htmlFor;return li(e??""),!e||!this.host.isConnected?null:this.resolveById(e)}return this.currentControl||this.host.parentElement}resolveById(e){return this.host.getRootNode().getElementById(e)}set control(e){e?this.attach(e):this.detach()}constructor(e,t){this.observer=null,this.currentControl=null,this.host=e,this.onControlChange=t,e.addController(this)}attach(e){e!==this.currentControl&&(this.setCurrentControl(e),this.host.removeAttribute("for"))}detach(){this.setCurrentControl(null),this.host.setAttribute("for","")}hostConnected(){!!1&&!this.observer&&(this.observer=new MutationObserver(()=>this.setCurrentControl(this.control)),this.observer.observe(this.host,{attributeFilter:["for"]})),this.setCurrentControl(this.control)}hostDisconnected(){this.observer?.disconnect(),this.observer=null,this.setCurrentControl(null)}setCurrentControl(e){e!==this.currentControl&&(this.onControlChange(this.currentControl,e),this.currentControl=e)}};var ci=class R{Key;Value;toJSON(){return{Key:this.Key,Value:this.Value}}ToCSSDeclaration({Semicolon:t=!1}={}){let r=`cubic-bezier(${this.Value[0]}, ${this.Value[1]}, ${this.Value[2]}, ${this.Value[3]})`;return`${this.Key}: ${r}${t?";":""}`}ToCSSVariable(){let t=`cubic-bezier(${this.Value[0]}, ${this.Value[1]}, ${this.Value[2]}, ${this.Value[3]})`;return`var(${this.Key}, ${t})`}ToCSSValue(){return`cubic-bezier(${this.Value[0]}, ${this.Value[1]}, ${this.Value[2]}, ${this.Value[3]})`}toString(){return this.ToCSSDeclaration()}constructor(t,r){this.Key=t,this.Value=r}static Standard=new R("--md-sys-motion-easing-standard",[.2,0,0,1]);static StandardAccelerate=new R("--md-sys-motion-easing-standard-accelerate",[.3,0,1,1]);static StandardDecelerate=new R("--md-sys-motion-easing-standard-decelerate",[0,0,0,1]);static Emphasized=new R("--md-sys-motion-easing-emphasized",[.2,0,0,1]);static EmphasizedAccelerate=new R("--md-sys-motion-easing-emphasized-accelerate",[.3,0,.8,.15]);static EmphasizedDecelerate=new R("--md-sys-motion-easing-emphasized-decelerate",[.05,.7,.1,1]);static Legacy=new R("--md-sys-motion-easing-legacy",[.4,0,.2,1]);static LegacyAccelerate=new R("--md-sys-motion-easing-legacy-accelerate",[.4,0,1,1]);static LegacyDecelerate=new R("--md-sys-motion-easing-legacy-decelerate",[0,0,.2,1]);static Linear=new R("--md-sys-motion-easing-linear",[0,0,1,1]);static ExpressiveFastSpatial=new R("--md-sys-motion-easing-expressive-fast-spatial",[.42,1.67,.21,.9]);static ExpressiveDefaultSpatial=new R("--md-sys-motion-easing-expressive-default-spatial",[.38,1.21,.22,1]);static ExpressiveSlowSpatial=new R("--md-sys-motion-easing-expressive-slow-spatial",[.39,1.29,.35,.98]);static ExpressiveFastEffects=new R("--md-sys-motion-easing-expressive-fast-effects",[.31,.94,.34,1]);static ExpressiveDefaultEffects=new R("--md-sys-motion-easing-expressive-default-effects",[.34,.8,.34,1]);static ExpressiveSlowEffects=new R("--md-sys-motion-easing-expressive-slow-effects",[.34,.88,.34,1]);static StandardFastSpatial=new R("--md-sys-motion-easing-standard-fast-spatial",[.27,1.06,.18,1]);static StandardDefaultSpatial=new R("--md-sys-motion-easing-standard-default-spatial",[.27,1.06,.18,1]);static StandardSlowSpatial=new R("--md-sys-motion-easing-standard-slow-spatial",[.27,1.06,.18,1]);static StandardFastEffects=new R("--md-sys-motion-easing-standard-fast-effects",[.31,.94,.34,1]);static StandardDefaultEffects=new R("--md-sys-motion-easing-standard-default-effects",[.34,.8,.34,1]);static StandardSlowEffects=new R("--md-sys-motion-easing-standard-slow-effects",[.34,.88,.34,1]);static AllEnums={Standard:this.Standard,StandardAccelerate:this.StandardAccelerate,StandardDecelerate:this.StandardDecelerate,Emphasized:this.Emphasized,EmphasizedAccelerate:this.EmphasizedAccelerate,EmphasizedDecelerate:this.EmphasizedDecelerate,Legacy:this.Legacy,LegacyAccelerate:this.LegacyAccelerate,LegacyDecelerate:this.LegacyDecelerate,Linear:this.Linear,ExpressiveFastSpatial:this.ExpressiveFastSpatial,ExpressiveDefaultSpatial:this.ExpressiveDefaultSpatial,ExpressiveSlowSpatial:this.ExpressiveSlowSpatial,ExpressiveFastEffects:this.ExpressiveFastEffects,ExpressiveDefaultEffects:this.ExpressiveDefaultEffects,ExpressiveSlowEffects:this.ExpressiveSlowEffects,StandardFastSpatial:this.StandardFastSpatial,StandardDefaultSpatial:this.StandardDefaultSpatial,StandardSlowSpatial:this.StandardSlowSpatial,StandardFastEffects:this.StandardFastEffects,StandardDefaultEffects:this.StandardDefaultEffects,StandardSlowEffects:this.StandardSlowEffects}};var I={Inactive:0,TouchDelay:1,Holding:2,WaitingForClick:3},lt={PressGrowMs:450,MinimumPressMs:225,InitialOriginScale:.2,Padding:10,SoftEdgeMinimumSize:75,SoftEdgeContainerRadio:.35,AnimationFill:"forwards",TouchDelayMs:150},Ao=["click","contextmenu","pointercancel","pointerdown","pointerenter","pointerleave","pointerup","focus","blur"],hi=class{constructor(e){this.state=I.Inactive,this.startEvent=null,this.checkBoundsAfterContextMenu=!1,this.initialSize=0,this.rippleScale="",this.rippleSize="",this.growAnimation=null,this.handleEvent=async t=>{switch(t.type){case"click":this.handleClick(t);break;case"contextmenu":this.handleContextmenu();break;case"pointercancel":this.handlePointercancel(t);break;case"pointerdown":await this.handlePointerdown(t);break;case"pointerenter":this.handlePointerenter(t);break;case"pointerleave":this.handlePointerleave(t);break;case"pointerup":this.handlePointerup(t);break;case"focus":this.handleFocus();break;case"blur":this.handleBlur()}},this.host=e}onControlChange(e,t){if(!!1)for(let r of Ao)e?.removeEventListener(r,this.handleEvent),t?.addEventListener(r,this.handleEvent)}handlePointerenter(e){this.shouldReactToEvent(e)&&(this.host.hovered=!0)}handlePointerleave(e){this.shouldReactToEvent(e)&&(this.host.hovered=!1,this.state!==I.Inactive&&this.endPressAnimation())}handlePointerup(e){if(this.shouldReactToEvent(e)){if(this.state===I.Holding){this.state=I.WaitingForClick;return}if(this.state===I.TouchDelay){this.state=I.WaitingForClick,this.startPressAnimation(this.startEvent||void 0);return}}}async handlePointerdown(e){if(this.shouldReactToEvent(e)){if(this.startEvent=e,!this.isTouch(e)){this.state=I.WaitingForClick,this.startPressAnimation(e);return}this.checkBoundsAfterContextMenu&&!this.inBounds(e)||(this.checkBoundsAfterContextMenu=!1,this.state=I.TouchDelay,await new Promise(t=>{setTimeout(t,lt.TouchDelayMs)}),this.state===I.TouchDelay&&(this.state=I.Holding,this.startPressAnimation(e)))}}handlePointercancel(e){this.shouldReactToEvent(e)&&this.endPressAnimation()}handleClick(e){if(this.host.disabled)return;if(this.state===I.WaitingForClick){this.endPressAnimation();return}let t=!e||e.detail===0;this.state===I.Inactive&&t&&(this.startPressAnimation(),this.endPressAnimation())}handleContextmenu(){this.host.disabled||(this.checkBoundsAfterContextMenu=!0,this.endPressAnimation())}handleFocus(){this.host.disabled||(this.host.focused=!0)}handleBlur(){this.host.disabled||(this.host.focused=!1,this.state!==I.Inactive&&this.endPressAnimation())}startPressAnimation(e){this.host.pressed=!0,this.growAnimation?.cancel(),this.determineRippleSize();let{startPoint:t,endPoint:r}=this.getTranslationCoordinates(e),i=`${t.x}px, ${t.y}px`,o=`${r.x}px, ${r.y}px`;this.host!==null&&(this.growAnimation=this.host.pressStateLayerElement.animate({top:[0,0],left:[0,0],height:[this.rippleSize,this.rippleSize],width:[this.rippleSize,this.rippleSize],transform:[`translate(${i}) scale(1)`,`translate(${o}) scale(${this.rippleScale})`]},{duration:lt.PressGrowMs,easing:ci.Standard.ToCSSValue(),fill:lt.AnimationFill}))}getTranslationCoordinates(e){if(this.host===null)return;let{height:t,width:r}=this.host.getBoundingClientRect(),i={x:(r-this.initialSize)/2,y:(t-this.initialSize)/2},o={x:r/2,y:t/2};return typeof PointerEvent<"u"&&e instanceof PointerEvent&&(o=this.getNormalizedPointerEventCoords(e)),o={x:o.x-this.initialSize/2,y:o.y-this.initialSize/2},{startPoint:o,endPoint:i}}getNormalizedPointerEventCoords(e){if(this.host===null)return;let{scrollX:t,scrollY:r}=window,{left:i,top:o}=this.host.getBoundingClientRect(),n=t+i,s=r+o,{pageX:l,pageY:a}=e;return{x:l-n,y:a-s}}async endPressAnimation(){this.startEvent=null,this.state=I.Inactive;let e=this.growAnimation,t=1/0;if(typeof e?.currentTime=="number"?t=e.currentTime:e?.currentTime&&(t=e.currentTime.to("ms").value),t>=lt.MinimumPressMs){this.host.pressed=!1;return}await new Promise(r=>{setTimeout(r,lt.MinimumPressMs-t)}),this.growAnimation===e&&(this.host.pressed=!1)}determineRippleSize(){if(this.host===null)return;let{height:e,width:t}=this.host.getBoundingClientRect(),r=Math.max(e,t),i=Math.max(lt.SoftEdgeContainerRadio*r,lt.SoftEdgeMinimumSize),o=Math.floor(r*lt.InitialOriginScale),n=Math.sqrt(t**2+e**2)+lt.Padding;this.initialSize=o,this.rippleScale=`${(n+i)/this.initialSize}`,this.rippleSize=`${this.initialSize}px`}inBounds({x:e,y:t}){if(this.host===null)return;let{top:r,left:i,bottom:o,right:n}=this.host.getBoundingClientRect();return e>=i&&e<=n&&t>=r&&t<=o}isTouch({pointerType:e}){return e==="touch"}shouldReactToEvent(e){if(this.host.disabled||!e.isPrimary||this.startEvent&&this.startEvent.pointerId!==e.pointerId)return!1;if(e.type==="pointerenter"||e.type==="pointerleave")return!this.isTouch(e);if(e.type==="pointerup"||e.type==="pointercancel")return!0;let t=e.buttons===1;return this.isTouch(e)||t}};var di=Kt({color:{enabled:"transparent",hovered:E.OnSurface,focused:E.OnSurface,pressed:E.OnSurface},opacity:{enabled:"0",hovered:ot.HoveredStateLayerOpacity,focused:ot.FocusedStateLayerOpacity,pressed:ot.PressedStateLayerOpacity}});var _o=Wt("--mdc-ripple")(di),$o=Ct`
    @layer mdc.ripple.component {
        :host {
            display: flex;
            margin: auto;
            pointer-events: none;
            border-radius: inherit;
            position: absolute;
            inset: 0;
            overflow: hidden;
            -webkit-tap-highlight-color: transparent;
        }

        .ripple {
            border-radius: inherit;
            position: absolute;
            inset: 0;
            overflow: hidden;
            -webkit-tap-highlight-color: transparent;
        }

        :host([disabled]),
        :host([disable-hover-state-layer]) .hover-state-layer,
        :host([disable-focus-state-layer]) .focus-state-layer,
        :host([disable-press-state-layer]) .press-state-layer {
            display: none;
        }

        :host,
        .hover-state-layer,
        .focus-state-layer,
        .press-state-layer {
            transition-behavior: allow-discrete;
            @starting-style {
                display: flex;
                opacity: 0;
            }
        }

        .hover-state-layer,
        .focus-state-layer,
        .press-state-layer {
            position: absolute;
            opacity: 0;
            border-radius: inherit;
            pointer-events: none;
            overflow: hidden;
            -webkit-tap-highlight-color: transparent;
            inset: 0;
        }

        .hover-state-layer {
            background-color: var(--_hovered-color);
            transition: opacity 200ms cubic-bezier(0.2, 0, 0, 1), background-color 200ms cubic-bezier(0.2, 0, 0, 1);
        }
        .focus-state-layer {
            background-color: var(--_focused-color);
            transition: opacity 150ms cubic-bezier(0.2, 0, 0, 1), background-color 150ms cubic-bezier(0.2, 0, 0, 1);
        }
        .press-state-layer {
            background: radial-gradient(closest-side, var(--_pressed-color) max(calc(100% - 70px), 65%), transparent 100%);
            transform-origin: center center;
            transition: opacity 375ms cubic-bezier(0.2, 0, 0, 1);
        }

        :host([hovered]:not([disable-hover-state-layer])) .hover-state-layer {
            opacity: var(--_hovered-opacity);
            transition-duration: 75ms;
        }
        :host([focused]:not([disable-focus-state-layer])) .focus-state-layer {
            opacity: var(--_focused-opacity);
            transition-duration: 75ms;
        }
        :host([pressed]:not([disable-press-state-layer])) .press-state-layer {
            opacity: var(--_pressed-opacity);
            transition-duration: 105ms;
        }
    }

    @layer mdc.ripple.motion {
        @media (prefers-reduced-motion: reduce) {
            :host,
            :host * {
                animation: none;
                transition: none;
            }
        }
    }

    @layer mdc.ripple.transparency {
        @media (prefers-reduced-transparency: reduce) {
            :host * {
            :host([hovered]:not([disable-hover-state-layer])) .hover-state-layer {
                opacity: 0.2;
            }
            :host([focused]:not([disable-focus-state-layer])) .focus-state-layer {
                opacity: 0.22;
            }
            :host([pressed]:not([disable-press-state-layer])) .press-state-layer {
                opacity: 0.22;
            }
            }
        }
    }

    @layer mdc.ripple.hcm {
        @media (forced-colors: active) {
            :host,
            .ripple {
                display: none;
            }
        }
    }

    @layer mdc.ripple.contrast {
        @media (prefers-contrast: more) {
            :host([hovered]:not([disable-hover-state-layer])) .hover-state-layer {
                opacity: 0.1;
            }
            :host([focused]:not([disable-focus-state-layer])) .focus-state-layer {
                opacity: 0.12;
            }
            :host([pressed]:not([disable-press-state-layer])) .press-state-layer {
                opacity: 0.12;
            }
        }

        @media (prefers-contrast: less) {
            :host([hovered]:not([disable-hover-state-layer])) .hover-state-layer {
                opacity: 0.05;
            }
            :host([focused]:not([disable-focus-state-layer])) .focus-state-layer {
                opacity: 0.08;
            }
            :host([pressed]:not([disable-press-state-layer])) .press-state-layer {
                opacity: 0.08;
            }
        }
    }
`,ui=[Ct`
        @layer mdc.icon {
            @layer variable, component, motion, hcm, contrast, transparency;
        }
        @layer mdc.ripple.variable{:host{${_o};}}
    `,$o];var Ce=e=>(t,r)=>{r!==void 0?r.addInitializer(()=>{customElements.define(e,t)}):customElements.define(e,t)};var Mo={attribute:!0,type:String,converter:Xt,reflect:!1,hasChanged:ne},ko=(e=Mo,t,r)=>{let{kind:i,metadata:o}=r,n=globalThis.litPropertyMetadata.get(o);if(n===void 0&&globalThis.litPropertyMetadata.set(o,n=new Map),i==="setter"&&((e=Object.create(e)).wrapped=!0),n.set(r.name,e),i==="accessor"){let{name:s}=r;return{set(l){let a=t.get.call(this);t.set.call(this,l),this.requestUpdate(s,a,e,!0,l)},init(l){return l!==void 0&&this.C(s,void 0,e,l),l}}}if(i==="setter"){let{name:s}=r;return function(l){let a=this[s];t.call(this,l),this.requestUpdate(s,a,e,!0,l)}}throw Error("Unsupported decorator location: "+i)};function C(e){return(t,r)=>typeof r=="object"?ko(e,t,r):((i,o,n)=>{let s=o.hasOwnProperty(n);return o.constructor.createProperty(n,i),s?Object.getOwnPropertyDescriptor(o,n):void 0})(e,t,r)}function St(e){return C({...e,state:!0,attribute:!1})}var kt=(e,t,r)=>(r.configurable=!0,r.enumerable=!0,Reflect.decorate&&typeof t!="object"&&Object.defineProperty(e,t,r),r);function G(e,t){return(r,i,o)=>{let n=s=>s.renderRoot?.querySelector(e)??null;if(t){let{get:s,set:l}=typeof i=="object"?r:o??(()=>{let a=Symbol();return{get(){return this[a]},set(c){this[a]=c}}})();return kt(r,i,{get(){let a=s.call(this);return a===void 0&&(a=n(this),(a!==null||this.hasUpdated)&&l.call(this,a)),a}})}return kt(r,i,{get(){return n(this)}})}}var W=class extends Q{constructor(...t){super(...t),this._ignoreGlobalConfig=!1,this._hasLocalConfig=!1,this._disabled=!1,this._disableHoverStateLayer=!1,this._disableFocusStateLayer=!1,this._disablePressStateLayer=!1,this._configAttributeSyncSource=null,this._lastEffectiveDisabled=!1,this.globalMDCContextConsumer=new zt(this,{context:xe,subscribe:!0}),this.action=new hi(this),this.attachableController=new pr(this,this.action.onControlChange.bind(this.action))}static{this.styles=ui}get ignoreGlobalConfig(){return this._ignoreGlobalConfig}set ignoreGlobalConfig(t){let r=this._ignoreGlobalConfig;r!==t&&(this._ignoreGlobalConfig=t,this._configAttributeSyncSource="property",this.toggleAttribute("ignore-global-config",t),this._configAttributeSyncSource=null,this.requestUpdate("ignoreGlobalConfig",r))}get disableHoverStateLayer(){return this.effectiveDisableHoverStateLayer}set disableHoverStateLayer(t){if(this._configAttributeSyncSource)return;let r=this._disableHoverStateLayer;this._disableHoverStateLayer=t,this._hasLocalConfig=!0,this._configAttributeSyncSource="property",this.toggleAttribute("disable-hover-state-layer",t),this._configAttributeSyncSource=null,this.requestUpdate("disableHoverStateLayer",r)}get disableFocusStateLayer(){return this.effectiveDisableFocusStateLayer}set disableFocusStateLayer(t){if(this._configAttributeSyncSource)return;let r=this._disableFocusStateLayer;this._disableFocusStateLayer=t,this._hasLocalConfig=!0,this._configAttributeSyncSource="property",this.toggleAttribute("disable-focus-state-layer",t),this._configAttributeSyncSource=null,this.requestUpdate("disableFocusStateLayer",r)}get disablePressStateLayer(){return this.effectiveDisablePressStateLayer}set disablePressStateLayer(t){if(this._configAttributeSyncSource)return;let r=this._disablePressStateLayer;this._disablePressStateLayer=t,this._hasLocalConfig=!0,this._configAttributeSyncSource="property",this.toggleAttribute("disable-press-state-layer",t),this._configAttributeSyncSource=null,this.requestUpdate("disablePressStateLayer",r)}get disabled(){return this.effectiveDisabled}set disabled(t){let r=this._disabled;this._configAttributeSyncSource||(this._disabled=t,this._hasLocalConfig=!0,this._configAttributeSyncSource="property",this.toggleAttribute("disabled",t),this._configAttributeSyncSource=null,this.requestUpdate("disabled",r))}get useLocalConfig(){return this._ignoreGlobalConfig||this._hasLocalConfig}get globalRippleConfig(){let t=this.globalMDCContextConsumer.value?.ripple;return{disabled:t?.disabled??!1,disableHoverStateLayer:t?.disableHoverStateLayer??!1,disableFocusStateLayer:t?.disableFocusStateLayer??!1,disablePressStateLayer:t?.disablePressStateLayer??!1}}get effectiveDisabled(){return this.useLocalConfig?this._disabled:this.globalRippleConfig.disabled}get effectiveDisableHoverStateLayer(){return this.useLocalConfig?this._disableHoverStateLayer:this.globalRippleConfig.disableHoverStateLayer}get effectiveDisableFocusStateLayer(){return this.useLocalConfig?this._disableFocusStateLayer:this.globalRippleConfig.disableFocusStateLayer}get effectiveDisablePressStateLayer(){return this.useLocalConfig?this._disablePressStateLayer:this.globalRippleConfig.disablePressStateLayer}syncConfigAttributes(){let t=(r,i)=>{this.hasAttribute(r)!==i&&(this._configAttributeSyncSource="effective",this.toggleAttribute(r,i),this._configAttributeSyncSource=null)};t("disabled",this.effectiveDisabled),t("disable-hover-state-layer",this.effectiveDisableHoverStateLayer),t("disable-focus-state-layer",this.effectiveDisableFocusStateLayer),t("disable-press-state-layer",this.effectiveDisablePressStateLayer)}render(){return V`<span aria-hidden="true" aria-disabled="${this.disabled}" class="ripple"><span class="hover-state-layer" aria-hidden="true"></span> <span class="focus-state-layer" aria-hidden="true"></span> <span class="press-state-layer" aria-hidden="true"></span></span>`}get hoverStateLayerElement(){return this._hoverStateLayerElement}get focusStateLayerElement(){return this._focusStateLayerElement}get pressStateLayerElement(){return this._pressStateLayerElement}get hovered(){return this.hasAttribute("hovered")}set hovered(t){this.toggleAttribute("hovered",t)}get focused(){return this.hasAttribute("focused")}set focused(t){this.toggleAttribute("focused",t)}get pressed(){return this.hasAttribute("pressed")}set pressed(t){this.toggleAttribute("pressed",t)}get htmlFor(){return this.attachableController.htmlFor}set htmlFor(t){this.attachableController.htmlFor=t}get control(){return this.attachableController.control}set control(t){this.attachableController.control=t}attach(t){this.attachableController.attach(t)}detach(){this.attachableController.detach()}updated(t){super.updated(t),this.syncConfigAttributes();let r=this.effectiveDisabled;r!==this._lastEffectiveDisabled&&(r&&(this.hovered=!1,this.focused=!1,this.pressed=!1),this._lastEffectiveDisabled=r)}};S([G(".ripple")],W.prototype,"rippleElement",void 0);S([G(".hover-state-layer")],W.prototype,"_hoverStateLayerElement",void 0);S([G(".focus-state-layer")],W.prototype,"_focusStateLayerElement",void 0);S([G(".press-state-layer")],W.prototype,"_pressStateLayerElement",void 0);S([C({type:Boolean,attribute:"ignore-global-config",reflect:!0,noAccessor:!0})],W.prototype,"ignoreGlobalConfig",null);S([C({type:Boolean,attribute:"disable-hover-state-layer",reflect:!0,noAccessor:!0})],W.prototype,"disableHoverStateLayer",null);S([C({type:Boolean,attribute:"disable-focus-state-layer",reflect:!0,noAccessor:!0})],W.prototype,"disableFocusStateLayer",null);S([C({type:Boolean,attribute:"disable-press-state-layer",reflect:!0,noAccessor:!0})],W.prototype,"disablePressStateLayer",null);S([C({type:Boolean,attribute:"disabled",reflect:!0,noAccessor:!0})],W.prototype,"disabled",null);W=S([Ce("mdc-ripple")],W);var Ro=Symbol("internals"),fr=Symbol("privateInternals");function pi(e){class t extends e{get[Ro](){return this[fr]||(this[fr]=this.attachInternals()),this[fr]}}return t}function fi(e){class t extends e{constructor(...i){super(...i),this._ignoreGlobalConfig=!1,this._focusRingDisabled=!1,this._focusRingDisabledConfigured=!1,this._focusRingAttributeSyncSource=null,this.focusRingShapeInherit=!0,this.focusRingInward=!1,this.focusRingAnimationDisabled=!1,this.focusRingPersistent=!1}get ignoreGlobalConfig(){return this._ignoreGlobalConfig}set ignoreGlobalConfig(i){let o=this._ignoreGlobalConfig;o!==i&&(this._ignoreGlobalConfig=i,this._focusRingAttributeSyncSource="property",this.toggleAttribute("ignore-global-config",i),this._focusRingAttributeSyncSource=null,this.requestUpdate("ignoreGlobalConfig",o))}get focusRingDisabled(){return this._focusRingDisabled}set focusRingDisabled(i){if(this._focusRingAttributeSyncSource)return;let o=this._focusRingDisabled,n=this._focusRingDisabledConfigured;this._focusRingDisabled=i,this._focusRingDisabledConfigured=!0,this._focusRingAttributeSyncSource="property",this.toggleAttribute("focus-ring-disabled",i),this._focusRingAttributeSyncSource=null,(o!==i||!n)&&this.requestUpdate("focusRingDisabled",o)}get focusRingHtmlFor(){return this.focusRingElement?.htmlFor??null}get focusRingControl(){return this.focusRingElement?.control??null}get useLocalConfig(){return this._ignoreGlobalConfig||this._focusRingDisabledConfigured}attributeChangedCallback(i,o,n){if(i==="ignore-global-config"){if(this._focusRingAttributeSyncSource)return;let s=this._ignoreGlobalConfig;this._ignoreGlobalConfig=n!==null,this.requestUpdate("ignoreGlobalConfig",s);return}if(i==="focus-ring-disabled"){if(this._focusRingAttributeSyncSource)return;let s=this._focusRingDisabled;this._focusRingDisabled=n!==null,this._focusRingDisabledConfigured=!0,this.requestUpdate("focusRingDisabled",s);return}super.attributeChangedCallback(i,o,n)}updated(i){super.updated(i);let o=this.focusRingElement;o&&(o.control!==this.focusRingControl&&(o.control=this.focusRingControl),o.htmlFor!==this.focusRingHtmlFor&&(o.htmlFor=this.focusRingHtmlFor))}renderFocusRing(){return this.useLocalConfig?V`<mdc-focus-ring part="focus-ring" id="focus-ring-part" ignore-global-config .disabled="${this.focusRingDisabled}" ?inward="${this.focusRingInward}" .shapeInherit="${this.focusRingShapeInherit}" .animationDisabled="${this.focusRingAnimationDisabled}" .persistent="${this.focusRingPersistent}"></mdc-focus-ring>`:V`<mdc-focus-ring part="focus-ring" id="focus-ring-part" ?inward="${this.focusRingInward}" .shapeInherit="${this.focusRingShapeInherit}" .animationDisabled="${this.focusRingAnimationDisabled}" .persistent="${this.focusRingPersistent}"></mdc-focus-ring>`}}return S([C({type:Boolean,reflect:!0,attribute:"ignore-global-config",noAccessor:!0})],t.prototype,"ignoreGlobalConfig",null),S([C({type:Boolean,reflect:!0,attribute:"focus-ring-shape-inherit"})],t.prototype,"focusRingShapeInherit",void 0),S([C({type:Boolean,reflect:!0,attribute:"focus-ring-inward"})],t.prototype,"focusRingInward",void 0),S([C({type:Boolean,attribute:"focus-ring-disabled",noAccessor:!0})],t.prototype,"focusRingDisabled",null),S([C({type:Boolean,reflect:!0,attribute:"focus-ring-animation-disabled"})],t.prototype,"focusRingAnimationDisabled",void 0),S([C({type:Boolean,reflect:!0,attribute:"focus-ring-persistent"})],t.prototype,"focusRingPersistent",void 0),S([G("#focus-ring-part")],t.prototype,"focusRingElement",void 0),t}function yi(e){class t extends e{constructor(...i){super(...i),this._ignoreGlobalConfig=!1,this._disableRipple=!1,this._disableRippleHoverStateLayer=!1,this._disableRippleFocusStateLayer=!1,this._disableRipplePressStateLayer=!1,this._rippleConfigConfigured=!1,this._rippleAttributeSyncSource=null}get ignoreGlobalConfig(){return this._ignoreGlobalConfig}set ignoreGlobalConfig(i){let o=this._ignoreGlobalConfig;o!==i&&(this._ignoreGlobalConfig=i,this._rippleAttributeSyncSource="property",this.toggleAttribute("ignore-global-config",i),this._rippleAttributeSyncSource&&(this._rippleAttributeSyncSource=null),this.requestUpdate("ignoreGlobalConfig",o))}get disableRipple(){return this._disableRipple}set disableRipple(i){if(this._rippleAttributeSyncSource)return;let o=this._disableRipple;this._disableRipple=i,this._rippleConfigConfigured=!0,this._rippleAttributeSyncSource="property",this.toggleAttribute("disable-ripple",i),this._rippleAttributeSyncSource=null,this.requestUpdate("disableRipple",o)}get disableRippleHoverStateLayer(){return this._disableRippleHoverStateLayer}set disableRippleHoverStateLayer(i){if(this._rippleAttributeSyncSource)return;let o=this._disableRippleHoverStateLayer;this._disableRippleHoverStateLayer=i,this._rippleConfigConfigured=!0,this._rippleAttributeSyncSource="property",this.toggleAttribute("disable-ripple-hover-state-layer",i),this._rippleAttributeSyncSource=null,this.requestUpdate("disableRippleHoverStateLayer",o)}get disableRippleFocusStateLayer(){return this._disableRippleFocusStateLayer}set disableRippleFocusStateLayer(i){if(this._rippleAttributeSyncSource)return;let o=this._disableRippleFocusStateLayer;this._disableRippleFocusStateLayer=i,this._rippleConfigConfigured=!0,this._rippleAttributeSyncSource="property",this.toggleAttribute("disable-ripple-focus-state-layer",i),this._rippleAttributeSyncSource=null,this.requestUpdate("disableRippleFocusStateLayer",o)}get disableRipplePressStateLayer(){return this._disableRipplePressStateLayer}set disableRipplePressStateLayer(i){if(this._rippleAttributeSyncSource)return;let o=this._disableRipplePressStateLayer;this._disableRipplePressStateLayer=i,this._rippleConfigConfigured=!0,this._rippleAttributeSyncSource="property",this.toggleAttribute("disable-ripple-press-state-layer",i),this._rippleAttributeSyncSource=null,this.requestUpdate("disableRipplePressStateLayer",o)}get useLocalConfig(){return this._ignoreGlobalConfig||this._rippleConfigConfigured}get rippleHtmlFor(){return this.rippleElement?.htmlFor??null}get rippleControl(){return this.rippleElement?.control??null}updated(i){super.updated(i),this.rippleElement&&(this.rippleElement.control!==this.rippleControl&&(this.rippleElement.control=this.rippleControl),this.rippleElement.htmlFor!==this.rippleHtmlFor&&(this.rippleElement.htmlFor=this.rippleHtmlFor))}renderRipple(){return this.useLocalConfig?V`<mdc-ripple part="ripple" id="ripple-part" ignore-global-config .disabled="${this.disableRipple}" .disableHoverStateLayer="${this.disableRippleHoverStateLayer}" .disableFocusStateLayer="${this.disableRippleFocusStateLayer}" .disablePressStateLayer="${this.disableRipplePressStateLayer}"></mdc-ripple>`:V`<mdc-ripple part="ripple" id="ripple-part"></mdc-ripple>`}}return S([C({type:Boolean,reflect:!0,attribute:"ignore-global-config",noAccessor:!0})],t.prototype,"ignoreGlobalConfig",null),S([C({type:Boolean,reflect:!0,attribute:"disable-ripple",noAccessor:!0})],t.prototype,"disableRipple",null),S([C({type:Boolean,reflect:!0,attribute:"disable-ripple-hover-state-layer",noAccessor:!0})],t.prototype,"disableRippleHoverStateLayer",null),S([C({type:Boolean,reflect:!0,attribute:"disable-ripple-focus-state-layer",noAccessor:!0})],t.prototype,"disableRippleFocusStateLayer",null),S([C({type:Boolean,reflect:!0,attribute:"disable-ripple-press-state-layer",noAccessor:!0})],t.prototype,"disableRipplePressStateLayer",null),S([G("#ripple-part")],t.prototype,"rippleElement",void 0),t}var Oo=Wt("--mdc-shape")(Be),To=Ct`
    @layer mdc.shape.variable {
        :host {
            --_resolved-size: var(--_medium-container-size);
            --_resolved-container-color: var(--_enabled-tonal-container-color);
            --_resolved-content-color: var(--_enabled-tonal-content-color);
            --_resolved-outline-color: transparent;
            --_resolved-outline-width: 0px;
            --_resolved-filter: none;
        }

        .container.size-extra-small {
            --_resolved-size: var(--_extra-small-container-size);
        }

        .container.size-small {
            --_resolved-size: var(--_small-container-size);
        }

        .container.size-medium {
            --_resolved-size: var(--_medium-container-size);
        }

        .container.size-large {
            --_resolved-size: var(--_large-container-size);
        }

        .container.size-extra-large {
            --_resolved-size: var(--_extra-large-container-size);
        }

        /* Color role re-keying (defaults = primary) */
        .container.color-secondary {
            --_enabled-filled-container-color: var(--_enabled-filled-container-color-secondary);
            --_enabled-filled-content-color: var(--_enabled-filled-content-color-secondary);
            --_enabled-tonal-container-color: var(--_enabled-tonal-container-color-secondary);
            --_enabled-tonal-content-color: var(--_enabled-tonal-content-color-secondary);
            --_enabled-outlined-outline-color: var(--_enabled-outlined-outline-color-secondary);
            --_enabled-elevated-content-color: var(--_enabled-elevated-content-color-secondary);
        }

        .container.color-tertiary {
            --_enabled-filled-container-color: var(--_enabled-filled-container-color-tertiary);
            --_enabled-filled-content-color: var(--_enabled-filled-content-color-tertiary);
            --_enabled-tonal-container-color: var(--_enabled-tonal-container-color-tertiary);
            --_enabled-tonal-content-color: var(--_enabled-tonal-content-color-tertiary);
            --_enabled-outlined-outline-color: var(--_enabled-outlined-outline-color-tertiary);
            --_enabled-elevated-content-color: var(--_enabled-elevated-content-color-tertiary);
        }

        .container.color-error {
            --_enabled-filled-container-color: var(--_enabled-filled-container-color-error);
            --_enabled-filled-content-color: var(--_enabled-filled-content-color-error);
            --_enabled-tonal-container-color: var(--_enabled-tonal-container-color-error);
            --_enabled-tonal-content-color: var(--_enabled-tonal-content-color-error);
            --_enabled-outlined-outline-color: var(--_enabled-outlined-outline-color-error);
            --_enabled-elevated-content-color: var(--_enabled-elevated-content-color-error);
        }

        .container.color-surface {
            --_enabled-filled-container-color: var(--_enabled-filled-container-color-surface);
            --_enabled-filled-content-color: var(--_enabled-filled-content-color-surface);
            --_enabled-tonal-container-color: var(--_enabled-tonal-container-color-surface);
            --_enabled-tonal-content-color: var(--_enabled-tonal-content-color-surface);
            --_enabled-outlined-outline-color: var(--_enabled-outlined-outline-color-surface);
            --_enabled-elevated-content-color: var(--_enabled-elevated-content-color-surface);
        }

        /* Variant selection */
        .container.variant-filled {
            --_resolved-container-color: var(--_enabled-filled-container-color);
            --_resolved-content-color: var(--_enabled-filled-content-color);
        }

        .container.variant-tonal {
            --_resolved-container-color: var(--_enabled-tonal-container-color);
            --_resolved-content-color: var(--_enabled-tonal-content-color);
        }

        .container.variant-outlined {
            --_resolved-container-color: var(--_enabled-outlined-container-color);
            --_resolved-content-color: var(--_enabled-outlined-content-color);
            --_resolved-outline-color: var(--_enabled-outlined-outline-color);
            --_resolved-outline-width: var(--_outline-width);
        }

        .container.variant-elevated {
            --_resolved-container-color: var(--_enabled-elevated-container-color);
            --_resolved-content-color: var(--_enabled-elevated-content-color);
            --_resolved-filter: drop-shadow(0 var(--_elevated-shadow-offset-y) var(--_elevated-shadow-blur) var(--_elevated-shadow-color));
        }

        .container.variant-surface {
            --_resolved-container-color: var(--_enabled-surface-container-color);
            --_resolved-content-color: var(--_enabled-surface-content-color);
        }

        .container.variant-transparent {
            --_resolved-container-color: var(--_enabled-transparent-container-color);
            --_resolved-content-color: var(--_enabled-transparent-content-color);
        }
    }

    @layer mdc.shape.base {
        :host {
            display: inline-flex;
            position: relative;
            box-sizing: border-box;
            width: var(--_resolved-size);
            height: var(--_resolved-size);
            vertical-align: middle;
            -webkit-tap-highlight-color: transparent;
            user-select: none;
        }

        :host([size='extra-small']) {
            --_resolved-size: var(--_extra-small-container-size);
        }

        :host([size='small']) {
            --_resolved-size: var(--_small-container-size);
        }

        :host([size='medium']) {
            --_resolved-size: var(--_medium-container-size);
        }

        :host([size='large']) {
            --_resolved-size: var(--_large-container-size);
        }

        :host([size='extra-large']) {
            --_resolved-size: var(--_extra-large-container-size);
        }

        :host([hidden]) {
            display: none;
        }

        .container {
            position: relative;
            display: inline-flex;
            align-items: center;
            justify-content: center;
            width: 100%;
            height: 100%;
            padding: 0;
            margin: 0;
            border: none;
            background: transparent;
            font: inherit;
            color: var(--_resolved-content-color);
            box-sizing: border-box;
            isolation: isolate;
            outline: none;
            overflow: visible;
            cursor: default;
            z-index: 0;
        }

        .container.interactive {
            cursor: pointer;
        }

        .container.disabled {
            cursor: default;
            pointer-events: none;
        }

        .shape-svg {
            position: absolute;
            inset: 0;
            width: 100%;
            height: 100%;
            overflow: visible;
            pointer-events: none;
            filter: var(--_resolved-filter);
            z-index: -1;
        }

        .shape-fill {
            fill: var(--_resolved-container-color);
            transition: fill 180ms ease;
        }

        .container.disabled .shape-fill {
            opacity: var(--_disabled-container-opacity);
        }

        .clip-layer {
            position: relative;
            display: flex;
            align-items: center;
            justify-content: center;
            width: 100%;
            height: 100%;
            overflow: hidden;
            z-index: 0;
        }

        .content {
            position: relative;
            display: inline-flex;
            align-items: center;
            justify-content: center;
            width: 100%;
            height: 100%;
            color: var(--_resolved-content-color);
            z-index: 0;
        }

        .container.disabled .content {
            opacity: var(--_disabled-content-opacity);
        }

        ::slotted(img),
        ::slotted(video),
        ::slotted(picture),
        ::slotted(canvas) {
            display: block;
            width: 100%;
            height: 100%;
            object-fit: cover;
        }

        .stroke-svg {
            position: absolute;
            inset: 0;
            width: 100%;
            height: 100%;
            overflow: visible;
            pointer-events: none;
            z-index: 1;
        }

        .shape-stroke {
            fill: none;
            stroke: var(--_resolved-outline-color);
            stroke-width: var(--_resolved-outline-width);
            transition: stroke 180ms ease;
        }

        .container.disabled .shape-stroke {
            opacity: var(--_disabled-content-opacity);
        }

        .shape-focus-stroke {
            fill: none;
            stroke: transparent;
            stroke-width: var(--_focus-outline-width);
            stroke-linejoin: round;
        }

        .container.interactive.focused-visible .shape-focus-stroke {
            stroke: var(--_focus-outline-color);
        }

        mdc-ripple {
            --mdc-ripple-hover-color: var(--_resolved-content-color);
            --mdc-ripple-pressed-color: var(--_resolved-content-color);
            --mdc-ripple-hover-opacity: var(--_hovered-state-layer-opacity);
            --mdc-ripple-pressed-opacity: var(--_pressed-state-layer-opacity);
            z-index: 1;
        }
    }

    @layer mdc.shape.a11y {
        @media (forced-colors: active) {
            .shape-fill {
                forced-color-adjust: none;
                fill: Canvas;
            }

            .shape-stroke {
                forced-color-adjust: none;
                stroke: CanvasText;
                stroke-width: 1.5px;
            }

            .content {
                color: CanvasText;
            }

            .container.interactive:hover .shape-stroke,
            .container.interactive.focused-visible .shape-stroke {
                stroke: Highlight;
                stroke-width: 2px;
            }

            .container.disabled .shape-stroke {
                stroke: GrayText;
            }

            .container.disabled .content {
                color: GrayText;
            }
        }

        @media (prefers-contrast: more) {
            .shape-stroke {
                stroke: currentColor;
                stroke-width: 1.5px;
            }
        }

        @media (prefers-contrast: less) {
            .shape-fill {
                opacity: 0.9;
            }
        }

        @media (prefers-reduced-motion: reduce) {
            .shape-fill,
            .shape-stroke {
                transition: none;
            }
        }

        @media (prefers-reduced-transparency: reduce) {
            .container.variant-elevated {
                --_resolved-filter: none;
            }
        }
    }
`,gi=[Ct`:host {${Oo};}`,To];var Fo=new Set(or),mi=new WeakMap,Do=1,Lo=e=>{let t=mi.get(e);return t===void 0&&(t=Do++,mi.set(e,t)),t},we=(e,t,r)=>Number.isFinite(e)&&e>=t?e:r,yr=(e,t)=>Number.isFinite(e)?Math.min(1,Math.max(0,e)):t,Vo=e=>Number.isFinite(e)?Math.min(.99,Math.max(.01,e)):.5,bi=(e,t)=>t.polygon!==null?`polygon-instance:${Lo(t.polygon)}`:t.vertices!==null&&t.vertices.length>=6?`vertices:${t.vertices.join(",")}:${t.cornerRadius}:${t.smoothing}`:e===H.Polygon||e===H.Star||e===H.Rectangle||e===H.PillStar?`parametric:${e}:${t.sides}:${t.innerRadius}:${t.cornerRadius}:${t.innerCornerRadius??"null"}:${t.smoothing}:${t.aspectRatio}:${t.vertexSpacing}`:`material:${e}`,Si=e=>{if(e.polygon!==null)return e.polygon.normalized();let t=m(we(e.cornerRadius,0,0),yr(e.smoothing,0));if(e.vertices!==null){if(e.vertices.length<6||e.vertices.length%2!==0)throw new Error("Custom vertices must contain an even number of at least 6 numbers.");return tt(e.vertices,t).normalized()}if(Fo.has(e.shape))return nr(e.shape);let r=Math.max(2,Math.round(we(e.sides,2,5))),i=Vo(e.innerRadius),o=e.innerCornerRadius!==null?m(we(e.innerCornerRadius,0,0),yr(e.smoothing,0)):null,n=we(e.aspectRatio,.05,1),s=yr(e.vertexSpacing,.5);switch(e.shape){case H.Polygon:return Mt(Math.max(3,r),1,0,0,t).normalized();case H.Star:return mt(r,1,i,t,o).normalized();case H.Rectangle:return Vt(n,1,t).normalized();case H.PillStar:return tr(n,1,r,i,t,o,null,s).normalized();default:throw new Error(`Unknown shape kind: "${String(e.shape)}"`)}},gr=e=>e.split(/[\s,]+/).map(t=>t.trim()).filter(t=>t.length>0);var Ee={ATTRIBUTE:1,CHILD:2,PROPERTY:3,BOOLEAN_ATTRIBUTE:4,EVENT:5,ELEMENT:6},Pe=e=>(...t)=>({_$litDirective$:e,values:t}),jt=class{constructor(t){}get _$AU(){return this._$AM._$AU}_$AT(t,r,i){this._$Ct=t,this._$AM=r,this._$Ci=i}_$AS(t,r){return this.update(t,r)}update(t,r){return this.render(...r)}};var vi=Pe(class extends jt{constructor(e){if(super(e),e.type!==Ee.ATTRIBUTE||e.name!=="class"||e.strings?.length>2)throw Error("`classMap()` can only be used in the `class` attribute and must be the only part in the attribute.")}render(e){return" "+Object.keys(e).filter(t=>e[t]).join(" ")+" "}update(e,[t]){if(this.st===void 0){this.st=new Set,e.strings!==void 0&&(this.nt=new Set(e.strings.join(" ").split(/\s/).filter(i=>i!=="")));for(let i in t)t[i]&&!this.nt?.has(i)&&this.st.add(i);return this.render(t)}let r=e.element.classList;for(let i of this.st)i in t||(r.remove(i),this.st.delete(i));for(let i in t){let o=!!t[i];o===this.st.has(i)||this.nt?.has(i)||(o?(r.add(i),this.st.add(i)):(r.remove(i),this.st.delete(i)))}return Y}});var xi="important",zo=" !"+xi,Ci=Pe(class extends jt{constructor(e){if(super(e),e.type!==Ee.ATTRIBUTE||e.name!=="style"||e.strings?.length>2)throw Error("The `styleMap` directive must be used in the `style` attribute and must be the only part in the attribute.")}render(e){return Object.keys(e).reduce((t,r)=>{let i=e[r];return i==null?t:t+`${r=r.includes("-")?r:r.replace(/(?:^(webkit|moz|ms|o)|)(?=[A-Z])/g,"-$&").toLowerCase()}:${i};`},"")}update(e,[t]){let{style:r}=e.element;if(this.ft===void 0)return this.ft=new Set(Object.keys(t)),this.render(t);for(let i of this.ft)t[i]==null&&(this.ft.delete(i),i.includes("-")?r.removeProperty(i):r[i]=null);for(let i in t){let o=t[i];if(o!=null){this.ft.add(i);let n=typeof o=="string"&&o.endsWith(zo);i.includes("-")||n?r.setProperty(i,n?o.slice(0,-11):o,n?xi:""):r[i]=o}}return Y}});var mr=[y.Circle,y.SoftBurst,y.Cookie9Sided,y.Pentagon,y.Pill,y.Sunny,y.Cookie4Sided,y.Heart],Io=8e-4,jo=.008,Ko=.05,qo=.5,Rt=()=>typeof window<"u"&&typeof document<"u",w=class extends si(ni,pi,yi,fi)(Q){constructor(...e){super(...e),this.shape="circle",this.hoverShape="",this.focusShape="",this.pressedShape="",this.selectedShape="",this.disabledShape="",this.morphTo="",this.progress=0,this.variant="tonal",this.color="primary",this.size="medium",this.fit="contain",this.rotation=0,this.startAngle=0,this.disableClip=!1,this.interactive=!1,this.toggle=!1,this.selected=!1,this.disabled=!1,this.motionScheme="expressive",this.motionSpeed="default",this.stiffness=null,this.dampingRatio=null,this.disableAnimation=!1,this.sides=5,this.innerRadius=.5,this.cornerRadius=.15,this.innerCornerRadius=null,this.smoothing=0,this.aspectRatio=1,this.vertexSpacing=.5,this.polygon=null,this.vertices=null,this.sequence="",this.autoplay=!1,this.interval=1e3,this.spinPerStep=0,this.isHovered=!1,this.isFocused=!1,this.isFocusVisible=!1,this.isPressed=!1,this.sequenceIndex=0,this.measuredWidth=0,this.measuredHeight=0,this.hasInitialized=!1,this.reducedMotion=!1,this.reducedMotionQuery=null,this.resizeObserver=null,this.rafId=null,this.autoplayTimerId=null,this.imperativeSpringOverride=null,this.settledPolygon=null,this.settledKey="",this.morphStartPolygon=null,this.morphEndPolygon=null,this.morphStartKey="",this.morphEndKey="",this.activeMorph=null,this.morphSpringStartVal=0,this.morphSpringStartVel=0,this.morphSpringTarget=1,this.morphStartTime=0,this.morphValue=1,this.morphVelocity=0,this.activeSpringParams=null,this.currentRotation=0,this.rotVelocity=0,this.rotStartVal=0,this.rotStartVel=0,this.rotTarget=0,this.rotStartTime=0,this.isRotationAnimating=!1,this.scrubKey="",this.scrubMorph=null,this.stepAnimation=t=>{this.rafId=null;let r=!1;if(this.activeMorph!==null){let i=this.activeSpringParams??this.getEffectiveSpring(),o=Math.max(0,t-this.morphStartTime),{value:n,velocity:s}=me(i)(this.morphSpringStartVal,this.morphSpringStartVel,o,this.morphSpringTarget);this.morphValue=n,this.morphVelocity=s,Math.abs(n-this.morphSpringTarget)<Io&&Math.abs(s)<jo?(this.settledPolygon=this.morphSpringTarget===1?this.morphEndPolygon:this.morphStartPolygon,this.settledKey=this.morphSpringTarget===1?this.morphEndKey:this.morphStartKey,this.activeMorph=null,this.morphValue=1,this.morphVelocity=0,this.dispatchMorphEnd(this.activeShape)):r=!0}if(this.isRotationAnimating){let i=this.getEffectiveSpring(),o=Math.max(0,t-this.rotStartTime),{value:n,velocity:s}=me(i)(this.rotStartVal,this.rotStartVel,o,this.rotTarget);this.currentRotation=n,this.rotVelocity=s,Math.abs(n-this.rotTarget)<Ko&&Math.abs(s)<qo?(this.currentRotation=this.rotTarget,this.rotVelocity=0,this.isRotationAnimating=!1):r=!0}this.requestUpdate(),r&&(this.rafId=requestAnimationFrame(this.stepAnimation))},this.handlePointerEnter=()=>{this.disabled||(this.isHovered=!0)},this.handlePointerLeave=()=>{this.isHovered=!1,this.isPressed=!1},this.handlePointerDown=t=>{this.disabled||t.button!==0||(this.isPressed=!0)},this.handlePointerUp=()=>{this.isPressed=!1},this.handlePointerCancel=()=>{this.isPressed=!1},this.handleFocus=()=>{this.disabled||(this.isFocused=!0,this.isFocusVisible=this.containerElement?.matches(":focus-visible")??!0)},this.handleBlur=()=>{this.isFocused=!1,this.isFocusVisible=!1,this.isPressed=!1},this.handleClick=t=>{if(this.disabled){t.preventDefault(),t.stopPropagation();return}this.toggle&&(this.selected=!this.selected,this.dispatchEvent(new CustomEvent(Ye,{detail:{selected:this.selected,activeShape:this.activeShape},bubbles:!0,composed:!0})))},this.handleReducedMotionChange=t=>{this.reducedMotion=t.matches,this.reducedMotion?(this.stopAnimationLoop(),this.stopAutoplay(),this.activeMorph=null,this.isRotationAnimating=!1,this.settledPolygon=this.resolvedPolygon,this.settledKey=this.getActiveTargetKey(),this.currentRotation=this.rotation,this.rotTarget=this.rotation,this.requestUpdate()):this.autoplay&&this.startAutoplay()}}static{this.styles=gi}get rippleControl(){return this.isInteractive&&!this.disabled?this.containerElement:null}get focusRingControl(){return null}get isInteractive(){return this.interactive||this.toggle}get activeStateOverride(){return this.disabled&&this.disabledShape?this.disabledShape:this.isPressed&&!this.disabled&&this.pressedShape?this.pressedShape:this.selected&&this.selectedShape?this.selectedShape:this.isFocused&&!this.disabled&&this.focusShape?this.focusShape:this.isHovered&&!this.disabled&&this.hoverShape?this.hoverShape:null}get activeShape(){return this.activeStateOverride??this.getSequenceShape()??this.shape}get resolvedPolygon(){let e=this.activeStateOverride===null&&this.getSequenceShape()===null;return this.resolvePolygonForKind(this.activeShape,e)}get isMorphing(){return this.activeMorph!==null||this.isRotationAnimating}get pathData(){let e=this.getCurrentCubics();return this.buildPixelPathData(e)}connectedCallback(){if(super.connectedCallback(),!!Rt()){if(typeof window.matchMedia=="function"){let e=window.matchMedia("(prefers-reduced-motion: reduce)");this.reducedMotionQuery=e,this.reducedMotion=e.matches,e.addEventListener("change",this.handleReducedMotionChange)}typeof ResizeObserver=="function"&&(this.resizeObserver=new ResizeObserver(e=>{let t=e[0];if(!t)return;let{width:r,height:i}=t.contentRect;(r!==this.measuredWidth||i!==this.measuredHeight)&&(this.measuredWidth=r,this.measuredHeight=i)}),this.resizeObserver.observe(this)),this.autoplay&&this.startAutoplay()}}disconnectedCallback(){super.disconnectedCallback(),Rt()&&(this.reducedMotionQuery?.removeEventListener("change",this.handleReducedMotionChange),this.reducedMotionQuery=null,this.resizeObserver?.disconnect(),this.resizeObserver=null,this.stopAnimationLoop(),this.stopAutoplay(),this.isHovered=!1,this.isPressed=!1,this.isFocused=!1,this.isFocusVisible=!1)}morphToShape(e,t){this.imperativeSpringOverride=t?.stiffness!==void 0||t?.dampingRatio!==void 0?{stiffness:t.stiffness??this.getEffectiveSpring().stiffness,dampingRatio:t.dampingRatio??this.getEffectiveSpring().dampingRatio}:null,e instanceof Lt?this.polygon=e.normalized():(this.polygon=null,this.vertices=null,this.shape=e),this.requestUpdate()}next(){let e=this.getEffectiveSequence();e.length!==0&&(this.sequenceIndex=(this.sequenceIndex+1)%e.length,this.spinPerStep!==0&&(this.rotation=this.rotation+this.spinPerStep))}willUpdate(e){if(super.willUpdate(e),e.has("disabled")&&this.disabled&&(this.isHovered=!1,this.isPressed=!1,this.isFocused=!1,this.isFocusVisible=!1),e.has("sequence")&&(this.sequenceIndex=0),this.morphTo){this.activeMorph=null,this.settledPolygon=this.resolvedPolygon,this.settledKey=this.getActiveTargetKey(),this.updateScrubMorph(),this.hasInitialized?e.has("rotation")&&this.handleRotationTargetChange():(this.currentRotation=this.rotation,this.rotTarget=this.rotation,this.hasInitialized=!0);return}let t=this.getActiveTargetKey(),r=this.resolvedPolygon;if(!this.hasInitialized){this.settledPolygon=r,this.settledKey=t,this.currentRotation=this.rotation,this.rotTarget=this.rotation,this.hasInitialized=!0;return}t!==this.getCurrentGoalKey()&&this.startOrRetargetMorph(t,r),e.has("rotation")&&this.rotation!==this.rotTarget&&this.handleRotationTargetChange()}updated(e){super.updated(e),(e.has("autoplay")||e.has("interval")||e.has("disableAnimation"))&&(this.autoplay&&!this.shouldSkipAnimation()?this.startAutoplay():this.stopAutoplay())}getRenderClasses(){return{container:!0,interactive:this.isInteractive,toggle:this.toggle,selected:this.selected,disabled:this.disabled,hovered:this.isHovered,focused:this.isFocused,"focused-visible":this.isFocusVisible,pressed:this.isPressed,morphing:this.isMorphing,[`variant-${this.variant}`]:!0,[`color-${this.color}`]:!0,[`size-${this.size}`]:!0}}render(){let{ariaLabel:e}=this,[t,r]=this.getBoxDimensions(),i=this.buildPixelPathData(this.getCurrentCubics(),t,r),o=this.disableClip?{}:{"clip-path":`path('${i}')`},n=vi(this.getRenderClasses()),s=V`<svg class="shape-svg" part="svg" viewBox="0 0 ${t} ${r}" aria-hidden="true"><path class="shape-fill" part="fill" d="${i}"/></svg><div class="clip-layer" part="clip" style="${Ci(o)}">${this.isInteractive&&!this.disabled?V`<mdc-ripple></mdc-ripple>`:M}<div class="content" part="content"><slot></slot></div></div><svg class="stroke-svg" part="stroke-svg" viewBox="0 0 ${t} ${r}" aria-hidden="true"><path class="shape-stroke" part="stroke" d="${i}"/>${this.isInteractive?V`<path class="shape-focus-stroke" part="focus-stroke" d="${i}">`:M}</svg>`;return this.isInteractive?V`<button id="container" part="container" type="button" class="${n}" ?disabled="${this.disabled}" aria-label="${e||M}" aria-pressed="${this.toggle?String(this.selected):M}" @pointerenter="${this.handlePointerEnter}" @pointerleave="${this.handlePointerLeave}" @pointerdown="${this.handlePointerDown}" @pointerup="${this.handlePointerUp}" @pointercancel="${this.handlePointerCancel}" @focus="${this.handleFocus}" @blur="${this.handleBlur}" @click="${this.handleClick}">${s}</button>`:V`<div id="container" part="container" class="${n}" role="${e?"img":M}" aria-label="${e||M}" @pointerenter="${this.handlePointerEnter}" @pointerleave="${this.handlePointerLeave}" @pointerdown="${this.handlePointerDown}" @pointerup="${this.handlePointerUp}" @pointercancel="${this.handlePointerCancel}">${s}</div>`}focus(e){if(Rt()&&this.isInteractive&&!this.disabled){this.containerElement?.focus(e);return}super.focus(e)}getSequenceShape(){let e=gr(this.sequence);return e.length>0?e[this.sequenceIndex%e.length]:this.autoplay?mr[this.sequenceIndex%mr.length]:null}getEffectiveSequence(){let e=gr(this.sequence);return e.length>0?e:mr}resolvePolygonForKind(e,t=!1){return Si({shape:e,polygon:t?this.polygon:null,vertices:t?this.vertices:null,sides:this.sides,innerRadius:this.innerRadius,cornerRadius:this.cornerRadius,innerCornerRadius:this.innerCornerRadius,smoothing:this.smoothing,aspectRatio:this.aspectRatio,vertexSpacing:this.vertexSpacing})}getActiveTargetKey(){let e=this.activeStateOverride===null&&this.getSequenceShape()===null;return bi(this.activeShape,{polygon:e?this.polygon:null,vertices:e?this.vertices:null,sides:this.sides,innerRadius:this.innerRadius,cornerRadius:this.cornerRadius,innerCornerRadius:this.innerCornerRadius,smoothing:this.smoothing,aspectRatio:this.aspectRatio,vertexSpacing:this.vertexSpacing})}getCurrentGoalKey(){return this.activeMorph!==null?this.morphSpringTarget===1?this.morphEndKey:this.morphStartKey:this.settledKey}shouldSkipAnimation(){return this.disableAnimation||this.reducedMotion}getEffectiveSpring(){if(this.imperativeSpringOverride!==null)return this.imperativeSpringOverride;let e=ar(this.motionScheme,this.motionSpeed);return{stiffness:this.stiffness!==null&&Number.isFinite(this.stiffness)&&this.stiffness>0?this.stiffness:e.stiffness,dampingRatio:this.dampingRatio!==null&&Number.isFinite(this.dampingRatio)&&this.dampingRatio>=0?this.dampingRatio:e.dampingRatio}}startOrRetargetMorph(e,t){if(this.shouldSkipAnimation()||!Rt()){this.activeMorph=null,this.settledPolygon=t,this.settledKey=e,this.morphValue=1,this.morphVelocity=0,this.imperativeSpringOverride=null;return}let r=this.getCurrentGoalKey();if(this.activeSpringParams=this.getEffectiveSpring(),this.imperativeSpringOverride=null,this.activeMorph!==null){if(e===this.morphStartKey&&this.morphSpringTarget!==0){this.morphSpringStartVal=this.morphValue,this.morphSpringStartVel=this.morphVelocity,this.morphSpringTarget=0,this.morphStartTime=performance.now(),this.dispatchMorphStart(r,e),this.ensureAnimationLoop();return}if(e===this.morphEndKey&&this.morphSpringTarget!==1){this.morphSpringStartVal=this.morphValue,this.morphSpringStartVel=this.morphVelocity,this.morphSpringTarget=1,this.morphStartTime=performance.now(),this.dispatchMorphStart(r,e),this.ensureAnimationLoop();return}let o=this.activeMorph.toPolygon(this.morphValue);this.morphStartPolygon=o,this.morphStartKey="__snapshot__",this.morphEndPolygon=t,this.morphEndKey=e,this.activeMorph=new Zt(o,t),this.morphSpringStartVal=0,this.morphSpringStartVel=0,this.morphSpringTarget=1,this.morphValue=0,this.morphVelocity=0,this.morphStartTime=performance.now(),this.dispatchMorphStart(r,e),this.ensureAnimationLoop();return}let i=this.settledPolygon??t;this.morphStartPolygon=i,this.morphStartKey=this.settledKey,this.morphEndPolygon=t,this.morphEndKey=e,this.activeMorph=new Zt(i,t),this.morphSpringStartVal=0,this.morphSpringStartVel=0,this.morphSpringTarget=1,this.morphValue=0,this.morphVelocity=0,this.morphStartTime=performance.now(),this.dispatchMorphStart(r,e),this.ensureAnimationLoop()}handleRotationTargetChange(){if(this.shouldSkipAnimation()||!Rt()){this.currentRotation=this.rotation,this.rotTarget=this.rotation,this.rotVelocity=0,this.isRotationAnimating=!1;return}this.rotStartVal=this.currentRotation,this.rotStartVel=this.rotVelocity,this.rotTarget=this.rotation,this.rotStartTime=performance.now(),this.isRotationAnimating=!0,this.ensureAnimationLoop()}updateScrubMorph(){if(!this.morphTo){this.scrubMorph=null,this.scrubKey="";return}let e=`${this.getActiveTargetKey()}->${`material:${this.morphTo}`}`;if(e!==this.scrubKey||this.scrubMorph===null){let t=this.resolvedPolygon,r=this.resolvePolygonForKind(this.morphTo);this.scrubMorph=new Zt(t,r),this.scrubKey=e}}getCurrentCubics(){return this.morphTo&&this.scrubMorph!==null?this.scrubMorph.asCubics(this.progress):this.activeMorph!==null?this.activeMorph.asCubics(this.morphValue):(this.settledPolygon??this.resolvedPolygon).cubics}getBoxDimensions(){let e=st[this.size]??st.medium;return[this.measuredWidth>0?this.measuredWidth:e,this.measuredHeight>0?this.measuredHeight:e]}buildPixelPathData(e,t,r){let[i,o]=t!==void 0&&r!==void 0?[t,r]:this.getBoxDimensions(),n=this.fit==="fill"?i:Math.min(i,o),s=this.fit==="fill"?o:Math.min(i,o);return rr(e,{rotation:this.currentRotation,startAngle:this.startAngle,rotationPivotX:.5,rotationPivotY:.5,sourceCenterX:.5,sourceCenterY:.5,scaleX:n,scaleY:s,centerX:i/2,centerY:o/2,precision:3})}ensureAnimationLoop(){this.rafId!==null||!Rt()||(this.rafId=requestAnimationFrame(this.stepAnimation))}stopAnimationLoop(){this.rafId!==null&&(cancelAnimationFrame(this.rafId),this.rafId=null)}startAutoplay(){if(this.stopAutoplay(),!Rt()||this.shouldSkipAnimation())return;let e=Number.isFinite(this.interval)&&this.interval>=100?this.interval:1e3;this.autoplayTimerId=window.setInterval(()=>{this.next()},e)}stopAutoplay(){this.autoplayTimerId!==null&&(window.clearInterval(this.autoplayTimerId),this.autoplayTimerId=null)}dispatchMorphStart(e,t){this.dispatchEvent(new CustomEvent(Xe,{detail:{from:e,to:t},bubbles:!0,composed:!0}))}dispatchMorphEnd(e){this.dispatchEvent(new CustomEvent(Ne,{detail:{shape:e},bubbles:!0,composed:!0}))}};S([C({type:String,reflect:!0})],w.prototype,"shape",void 0);S([C({type:String,attribute:"hover-shape",reflect:!0})],w.prototype,"hoverShape",void 0);S([C({type:String,attribute:"focus-shape",reflect:!0})],w.prototype,"focusShape",void 0);S([C({type:String,attribute:"pressed-shape",reflect:!0})],w.prototype,"pressedShape",void 0);S([C({type:String,attribute:"selected-shape",reflect:!0})],w.prototype,"selectedShape",void 0);S([C({type:String,attribute:"disabled-shape",reflect:!0})],w.prototype,"disabledShape",void 0);S([C({type:String,attribute:"morph-to",reflect:!0})],w.prototype,"morphTo",void 0);S([C({type:Number})],w.prototype,"progress",void 0);S([C({type:String,reflect:!0})],w.prototype,"variant",void 0);S([C({type:String,reflect:!0})],w.prototype,"color",void 0);S([C({type:String,reflect:!0})],w.prototype,"size",void 0);S([C({type:String,reflect:!0})],w.prototype,"fit",void 0);S([C({type:Number,reflect:!0})],w.prototype,"rotation",void 0);S([C({type:Number,attribute:"start-angle",reflect:!0})],w.prototype,"startAngle",void 0);S([C({type:Boolean,attribute:"disable-clip",reflect:!0})],w.prototype,"disableClip",void 0);S([C({type:Boolean,reflect:!0})],w.prototype,"interactive",void 0);S([C({type:Boolean,reflect:!0})],w.prototype,"toggle",void 0);S([C({type:Boolean,reflect:!0})],w.prototype,"selected",void 0);S([C({type:Boolean,reflect:!0})],w.prototype,"disabled",void 0);S([C({type:String,attribute:"motion-scheme",reflect:!0})],w.prototype,"motionScheme",void 0);S([C({type:String,attribute:"motion-speed",reflect:!0})],w.prototype,"motionSpeed",void 0);S([C({type:Number})],w.prototype,"stiffness",void 0);S([C({type:Number,attribute:"damping-ratio"})],w.prototype,"dampingRatio",void 0);S([C({type:Boolean,attribute:"disable-animation",reflect:!0})],w.prototype,"disableAnimation",void 0);S([C({type:Number})],w.prototype,"sides",void 0);S([C({type:Number,attribute:"inner-radius"})],w.prototype,"innerRadius",void 0);S([C({type:Number,attribute:"corner-radius"})],w.prototype,"cornerRadius",void 0);S([C({type:Number,attribute:"inner-corner-radius"})],w.prototype,"innerCornerRadius",void 0);S([C({type:Number})],w.prototype,"smoothing",void 0);S([C({type:Number,attribute:"aspect-ratio"})],w.prototype,"aspectRatio",void 0);S([C({type:Number,attribute:"vertex-spacing"})],w.prototype,"vertexSpacing",void 0);S([C({attribute:!1})],w.prototype,"polygon",void 0);S([C({attribute:!1})],w.prototype,"vertices",void 0);S([C({type:String})],w.prototype,"sequence",void 0);S([C({type:Boolean,reflect:!0})],w.prototype,"autoplay",void 0);S([C({type:Number})],w.prototype,"interval",void 0);S([C({type:Number,attribute:"spin-per-step"})],w.prototype,"spinPerStep",void 0);S([St()],w.prototype,"isHovered",void 0);S([St()],w.prototype,"isFocused",void 0);S([St()],w.prototype,"isFocusVisible",void 0);S([St()],w.prototype,"isPressed",void 0);S([St()],w.prototype,"sequenceIndex",void 0);S([St()],w.prototype,"measuredWidth",void 0);S([St()],w.prototype,"measuredHeight",void 0);S([G("#container")],w.prototype,"containerElement",void 0);var br=class extends w{};br=S([Ce("mdc-shape")],br);export{or as AllMaterialShapeNames,w as BaseShape,z as Cubic,Wr as DoubleMapper,Ue as Feature,q as FeatureCorner,Qt as FeatureEdge,ue as LengthMeasurer,br as MDCShape,y as MaterialShape,P as MaterialShapes,Zr as MeasuredCubic,pe as MeasuredPolygon,Zt as Morph,sr as MotionScheme,H as ParametricShape,Lt as RoundedPolygon,Ye as ShapeChangeEvent,Bi as ShapeColor,Be as ShapeDefinition,Ni as ShapeFit,Ki as ShapeKind,Ne as ShapeMorphEndEvent,Xe as ShapeMorphStartEvent,Yi as ShapeMotionScheme,Hi as ShapeMotionSpeed,Xi as ShapeSize,st as ShapeSizePixels,qi as ShapeVariant,U as Unrounded,fe as circlePolygon,m as cornerRounding,er as cubicsAreaCentroid,ei as cubicsBoundsCenter,rr as cubicsToPathData,Jr as doMapping,fo as estimateAnimationDurationMillis,Qr as featureDistSquared,Je as featureMapper,We as featureRepresentativePoint,nr as getMaterialShapePolygon,Ge as linearMap,eo as pillPolygon,tr as pillStarPolygon,g as point,Vt as rectanglePolygon,ar as resolveMotionSpring,Mt as roundedPolygonFromNumVertices,tt as roundedPolygonFromVertices,me as springMotionAt,mt as starPolygon};
/*! Bundled license information:

@sandlada/mdc/build/utils/color/color.js:
@sandlada/mdc/build/utils/color/index.js:
@sandlada/mdc/build/utils/style/internal/shape-helpers.js:
@sandlada/mdc/build/utils/style/expand-shape.js:
@sandlada/mdc/build/utils/style/internal/resolve-padding.js:
@sandlada/mdc/build/utils/style/internal/resolve-margin.js:
@sandlada/mdc/build/utils/style/internal/typescale-helpers.js:
@sandlada/mdc/build/components/shape/shape.definition.js:
@sandlada/mdc/build/components/shape/shape.interface.js:
@sandlada/mdc/build/utils/context-provider/context-provider.js:
@sandlada/mdc/build/utils/context-provider/index.js:
@sandlada/mdc/build/utils/attachment/index.js:
@sandlada/mdc/build/components/focus-ring/focus-ring-options.mixin.js:
@sandlada/mdc/build/components/ripple/ripple-options.mixin.js:
@sandlada/mdc/build/components/shape/shape.style.js:
@sandlada/mdc/build/components/shape/internal/shape-resolver.js:
@sandlada/mdc/build/components/shape/internal/base-shape.js:
@sandlada/mdc/build/components/shape/shape.js:
@sandlada/mdc/build/components/shape/index.js:
  (**
  * @license
  * Copyright 2026 Kai-Orion & Sandlada
  * SPDX-License-Identifier: MIT
  *)

@sandlada/mdc/build/utils/style/create-style-definition.js:
  (**
  * @license
  * Copyright 2026 Kai-Orion & Sandlada
  * SPDX-License-Identifier: MIT
  *
  * @fileoverview
  * Plain token definition factory for `utils/style`.
  *
  * Single-parameter only: accepts a token record and returns a frozen
  * definition. No schema, no Cartesian expansion, no at-rule metadata —
  * that logic stays in `@sandlada/styles`.
  *)

@sandlada/mdc/build/utils/style/css-like.js:
  (**
  * @license
  * Copyright 2026 Kai-Orion & Sandlada
  * SPDX-License-Identifier: MIT
  *
  * @fileoverview
  * Framework-agnostic CSS value contracts for `utils/style`.
  *
  * Mirrors the pure token layer of `@sandlada/styles` without the
  * schema / at-rule compiler. Lit's `CSSResult` structurally satisfies
  * `CSSLike` (`{ cssText }`).
  *)

@sandlada/mdc/build/utils/style/expand-padding.js:
  (**
  * @license
  * Copyright 2026 Kai-Orion & Sandlada
  * SPDX-License-Identifier: MIT
  *
  * @fileoverview
  * Pure functional token transformer expanding padding definitions into 4 CSS logical edge properties:
  * - `${prefix}-padding-block-start`
  * - `${prefix}-padding-block-end`
  * - `${prefix}-padding-inline-start`
  * - `${prefix}-padding-inline-end`
  *)

@sandlada/mdc/build/utils/style/expand-margin.js:
  (**
  * @license
  * Copyright 2026 Kai-Orion & Sandlada
  * SPDX-License-Identifier: MIT
  *
  * @fileoverview
  * Pure functional token transformer expanding margin definitions into 4 CSS logical edge properties:
  * - `${prefix}-margin-block-start`
  * - `${prefix}-margin-block-end`
  * - `${prefix}-margin-inline-start`
  * - `${prefix}-margin-inline-end`
  *)

@sandlada/mdc/build/utils/style/expand-typescale.js:
  (**
  * @license
  * Copyright 2026 Kai-Orion & Sandlada
  * SPDX-License-Identifier: MIT
  *
  * @fileoverview
  * Pure functional token transformer expanding Typescale definitions into 5 typographic token properties:
  * - `${prefix}-font`
  * - `${prefix}-leading`
  * - `${prefix}-size`
  * - `${prefix}-tracking`
  * - `${prefix}-weight`
  *)

@lit/reactive-element/css-tag.js:
  (**
   * @license
   * Copyright 2019 Google LLC
   * SPDX-License-Identifier: BSD-3-Clause
   *)

@lit/reactive-element/reactive-element.js:
lit-html/lit-html.js:
lit-element/lit-element.js:
@lit/context/lib/decorators/provide.js:
@lit/reactive-element/decorators/custom-element.js:
@lit/reactive-element/decorators/property.js:
@lit/reactive-element/decorators/state.js:
@lit/reactive-element/decorators/event-options.js:
@lit/reactive-element/decorators/base.js:
@lit/reactive-element/decorators/query.js:
@lit/reactive-element/decorators/query-all.js:
@lit/reactive-element/decorators/query-async.js:
@lit/reactive-element/decorators/query-assigned-nodes.js:
lit-html/directive.js:
  (**
   * @license
   * Copyright 2017 Google LLC
   * SPDX-License-Identifier: BSD-3-Clause
   *)

lit-html/is-server.js:
@lit/context/lib/decorators/consume.js:
  (**
   * @license
   * Copyright 2022 Google LLC
   * SPDX-License-Identifier: BSD-3-Clause
   *)

@sandlada/mdc/build/utils/style/stringify-tokens.js:
  (**
  * @license
  * Copyright 2026 Kai-Orion & Sandlada
  * SPDX-License-Identifier: MIT
  *
  * @fileoverview
  * Token stringifier for `utils/style`: emits `:host` private (`--_*`)
  * declarations with public (`--mdc-<prefix>-*`) override fallbacks.
  *
  * Simplified port without schema / Cartesian logic: static primitives emit
  * one declaration each, plain record values emit one declaration per entry.
  * Array values fail fast — the simplified system holds no schema to name
  * positional entries with.
  *)

@sandlada/mdc/build/utils/style/override-tokens.js:
  (**
  * @license
  * Copyright 2026 Kai-Orion & Sandlada
  * SPDX-License-Identifier: MIT
  *
  * @fileoverview
  * Token override helpers for `utils/style`: emit public
  * (`--mdc-<prefix>-*`) override declarations, optionally wrapped in a
  * selector. Pure string building without schema / at-rule logic.
  *)

@sandlada/mdc/build/utils/style/index.js:
  (**
  * @license
  * Copyright 2026 Kai-Orion & Sandlada
  * SPDX-License-Identifier: MIT
  *
  * @fileoverview
  * Local token layer for `@sandlada/mdc`: plain `createStyleDefinition`,
  * shape / padding / margin / typescale expanders, and Lit-bound
  * `stringifyTokens` / `overrideTokens`.
  *
  * Simplified port without schema / Cartesian / at-rule compiler logic.
  * Import from here inside `packages/mdc`; the original `@sandlada/styles`
  * files stay untouched.
  *)

@sandlada/mdc/build/components/shape/internal/point.js:
  (**
  * @license
  * Copyright 2022 The Android Open Source Project
  * SPDX-License-Identifier: Apache-2.0
  *
  * [Modified By Kai-Orion & Sandlada]
  *
  * Copyright 2026 Kai-Orion & Sandlada
  * SPDX-License-Identifier: MIT
  *
  * @fileoverview
  * TypeScript port of `androidx.graphics.shapes` point and scalar primitives
  * (`Point.kt` + `Utils.kt`). Pure functions only; every value is immutable.
  *
  * @link https://android.googlesource.com/platform/frameworks/support/+/androidx-main/graphics/graphics-shapes/src/commonMain/kotlin/androidx/graphics/shapes/Point.kt
  * @link https://android.googlesource.com/platform/frameworks/support/+/androidx-main/graphics/graphics-shapes/src/commonMain/kotlin/androidx/graphics/shapes/Utils.kt
  *)

@sandlada/mdc/build/components/shape/internal/cubic.js:
  (**
  * @license
  * Copyright 2022 The Android Open Source Project
  * SPDX-License-Identifier: Apache-2.0
  *
  * [Modified By Kai-Orion & Sandlada]
  *
  * Copyright 2026 Kai-Orion & Sandlada
  * SPDX-License-Identifier: MIT
  *
  * @fileoverview
  * TypeScript port of `androidx.graphics.shapes.Cubic`: a single cubic Bezier
  * curve (two anchors + two controls), the atom of the morphing-shape engine.
  * Instances are immutable; point data lives in a plain `Float64Array(8)`.
  *
  * @link https://android.googlesource.com/platform/frameworks/support/+/androidx-main/graphics/graphics-shapes/src/commonMain/kotlin/androidx/graphics/shapes/Cubic.kt
  *)

@sandlada/mdc/build/components/shape/internal/feature.js:
  (**
  * @license
  * Copyright 2023 The Android Open Source Project
  * SPDX-License-Identifier: Apache-2.0
  *
  * [Modified By Kai-Orion & Sandlada]
  *
  * Copyright 2026 Kai-Orion & Sandlada
  * SPDX-License-Identifier: MIT
  *
  * @fileoverview
  * TypeScript port of `androidx.graphics.shapes.Features`: features group the
  * raw cubics of a polygon into straight edges and (convex / concave) corners,
  * which is the granularity the morph mapping operates on.
  *
  * @link https://android.googlesource.com/platform/frameworks/support/+/androidx-main/graphics/graphics-shapes/src/commonMain/kotlin/androidx/graphics/shapes/Features.kt
  *)

@sandlada/mdc/build/components/shape/internal/feature-mapping.js:
  (**
  * @license
  * Copyright 2023 The Android Open Source Project
  * SPDX-License-Identifier: Apache-2.0
  *
  * [Modified By Kai-Orion & Sandlada]
  *
  * Copyright 2026 Kai-Orion & Sandlada
  * SPDX-License-Identifier: MIT
  *
  * @fileoverview
  * TypeScript port of `androidx.graphics.shapes.FeatureMapping` and
  * `FloatMapping`: maps the features (rounded corners) of two shapes onto each
  * other, producing a progress-to-progress `DoubleMapper` that `Morph` uses
  * to align the two outlines.
  *
  * @link https://android.googlesource.com/platform/frameworks/support/+/androidx-main/graphics/graphics-shapes/src/commonMain/kotlin/androidx/graphics/shapes/FeatureMapping.kt
  * @link https://android.googlesource.com/platform/frameworks/support/+/androidx-main/graphics/graphics-shapes/src/commonMain/kotlin/androidx/graphics/shapes/FloatMapping.kt
  *)

@sandlada/mdc/build/components/shape/internal/measure.js:
  (**
  * @license
  * Copyright 2023 The Android Open Source Project
  * SPDX-License-Identifier: Apache-2.0
  *
  * [Modified By Kai-Orion & Sandlada]
  *
  * Copyright 2026 Kai-Orion & Sandlada
  * SPDX-License-Identifier: MIT
  *
  * @fileoverview
  * TypeScript port of `androidx.graphics.shapes.PolygonMeasure`: measures cubics
  * along the outline (approximated by arc length) and can cut + shift a polygon
  * so a given outline progress becomes the new start.
  *
  * @link https://android.googlesource.com/platform/frameworks/support/+/androidx-main/graphics/graphics-shapes/src/commonMain/kotlin/androidx/graphics/shapes/PolygonMeasure.kt
  *)

@sandlada/mdc/build/components/shape/internal/rounded-polygon.js:
  (**
  * @license
  * Copyright 2022 The Android Open Source Project
  * SPDX-License-Identifier: Apache-2.0
  *
  * [Modified By Kai-Orion & Sandlada]
  *
  * Copyright 2026 Kai-Orion & Sandlada
  * SPDX-License-Identifier: MIT
  *
  * @fileoverview
  * TypeScript port of `androidx.graphics.shapes.RoundedPolygon` and
  * `Shapes.kt` (`circle`, `rectangle`, `star`, `pill`, `pillStar`).
  *
  * @link https://android.googlesource.com/platform/frameworks/support/+/androidx-main/graphics/graphics-shapes/src/commonMain/kotlin/androidx/graphics/shapes/RoundedPolygon.kt
  * @link https://android.googlesource.com/platform/frameworks/support/+/androidx-main/graphics/graphics-shapes/src/commonMain/kotlin/androidx/graphics/shapes/Shapes.kt
  *)

@sandlada/mdc/build/components/shape/internal/morph.js:
  (**
  * @license
  * Copyright 2022 The Android Open Source Project
  * SPDX-License-Identifier: Apache-2.0
  *
  * [Modified By Kai-Orion & Sandlada]
  *
  * Copyright 2026 Kai-Orion & Sandlada
  * SPDX-License-Identifier: MIT
  *
  * @fileoverview
  * TypeScript port of `androidx.graphics.shapes.Morph` and `ShapeUtil.kt` path
  * serialization, extended with `toPolygon(progress)` for seamless mid-flight
  * morph interruption.
  *
  * @link https://android.googlesource.com/platform/frameworks/support/+/androidx-main/graphics/graphics-shapes/src/commonMain/kotlin/androidx/graphics/shapes/Morph.kt
  * @link https://android.googlesource.com/platform/frameworks/support/+/androidx-main/compose/material3/material3/src/commonMain/kotlin/androidx/compose/material3/internal/ShapeUtil.kt
  *)

@sandlada/mdc/build/components/shape/internal/material-shapes.js:
  (**
  * @license
  * Copyright 2024 The Android Open Source Project
  * SPDX-License-Identifier: Apache-2.0
  *
  * [Modified By Kai-Orion & Sandlada]
  *
  * Copyright 2026 Kai-Orion & Sandlada
  * SPDX-License-Identifier: MIT
  *
  * @fileoverview
  * Complete 1:1 TypeScript port of `androidx.compose.material3.MaterialShapes`
  * (all 35 Material Design 3 Expressive predefined shapes), normalized into the
  * `[0, 1] x [0, 1]` unit square.
  *
  * @link https://android.googlesource.com/platform/frameworks/support/+/androidx-main/compose/material3/material3/src/commonMain/kotlin/androidx/compose/material3/MaterialShapes.kt
  *)

@sandlada/mdc/build/components/shape/internal/spring.js:
  (**
  * @license
  * Copyright 2019 The Android Open Source Project
  * SPDX-License-Identifier: Apache-2.0
  *
  * [Modified By Kai-Orion & Sandlada]
  *
  * Copyright 2026 Kai-Orion & Sandlada
  * SPDX-License-Identifier: MIT
  *
  * @fileoverview
  * TypeScript port of `androidx.compose.animation.core.SpringSimulation`,
  * `estimateAnimationDurationMillis`, and `androidx.compose.material3.MotionScheme`
  * (`ExpressiveMotionTokens` and `StandardMotionTokens`).
  *
  * @link https://android.googlesource.com/platform/frameworks/support/+/androidx-main/compose/animation/animation-core/src/commonMain/kotlin/androidx/compose/animation/core/SpringSimulation.kt
  * @link https://android.googlesource.com/platform/frameworks/support/+/androidx-main/compose/material3/material3/src/commonMain/kotlin/androidx/compose/material3/MotionScheme.kt
  *)

@sandlada/mdc/build/utils/aria/aria.js:
@sandlada/mdc/build/utils/aria/delegate.js:
@sandlada/mdc/build/utils/behaviors/element-internals.js:
  (**
  * @license
  * Copyright 2023 Google LLC
  * SPDX-License-Identifier: Apache-2.0
  *)

@sandlada/mdc/build/utils/compose-mixin/compose-mixin.js:
@sandlada/mdc/build/components/ripple/ripple.style.js:
  (**
  * @license
  * Copyright 2025 Kai-Orion & Sandlada
  * SPDX-License-Identifier: MIT
  *)

@lit/context/lib/context-request-event.js:
@lit/context/lib/create-context.js:
@lit/context/lib/controllers/context-consumer.js:
@lit/context/lib/value-notifier.js:
@lit/context/lib/controllers/context-provider.js:
@lit/context/lib/context-root.js:
@lit/reactive-element/decorators/query-assigned-elements.js:
  (**
   * @license
   * Copyright 2021 Google LLC
   * SPDX-License-Identifier: BSD-3-Clause
   *)

@sandlada/mdc/build/utils/attachment/attachable-controller.js:
  (**
  * @license
  * Copyright 2023 Google LLC
  * SPDX-License-Identifier: Apache-2.0
  * 
  * @link
  * https://github.com/material-components/material-web/blob/main/internal/controller/attachable-controller.ts
  *)

@sandlada/mdc/build/components/ripple/ripple-action.js:
  (**
  * @license
  * Copyright 2022 Google LLC
  * SPDX-License-Identifier: Apache-2.0
  * @link
  * https://github.com/material-components/material-web/blob/main/ripple/internal/ripple.ts
  *
  * [Modified by Kai-Orion & Sandlada]
  *)

@sandlada/mdc/build/components/ripple/ripple.definition.js:
  (**
  * @license
  * Copyright 2025 Kai-Orion & Sandlada
  * SPDX-License-Identifier: MIT
  *
  * @version
  * 1.0.0
  *)

@sandlada/mdc/build/components/ripple/ripple.js:
  (**
  * @license
  * Copyright 2025 Sandlada & Kai Orion
  * SPDX-License-Identifier: MIT
  *)

lit-html/directives/class-map.js:
lit-html/directives/style-map.js:
  (**
   * @license
   * Copyright 2018 Google LLC
   * SPDX-License-Identifier: BSD-3-Clause
   *)
*/
