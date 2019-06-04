!function (e, t) {
    "object" == typeof exports && "object" == typeof module ? module.exports = t() : "function" == typeof define && define.amd ? define("VNumber", t) : "object" == typeof exports ? exports.VNumber = t() : e.VNumber = t()
}

    ("undefined" != typeof self ? self : this, function (e) {
        return function (e) {
            function t(n) {
                if (r[n]) return r[n].exports;
                var i = r[n] = {
                    i: n, l: !1, exports: {}
                }
                    ;
                return e[n].call(i.exports, i, i.exports, t), i.l = !0, i.exports
            }
            var r = {}
                ;
            return t.m = e, t.c = r, t.d = function (e, r, n) {
                t.o(e, r) || Object.defineProperty(e, r, {
                    configurable: !1, enumerable: !0, get: n
                }
                )
            }
                , t.n = function (e) {
                    var r = e && e.__esModule ? function () {
                        return e.default
                    }
                        : function () {
                            return e
                        }
                        ;
                    return t.d(r, "a", r), r
                }
                , t.o = function (e, t) {
                    return Object.prototype.hasOwnProperty.call(e, t)
                }
                , t.p = "", t(t.s = 1)
        }
            ([function (e, t, r) {
                "use strict";
                var n = r(4), i = r.n(n);
                t.a = {
                    name: "VNumber", props: {
                        currency: {
                            type: String, default: "", required: !1
                        }
                        , max: {
                            type: Number, default: Number.MAX_SAFE_INTEGER || 9007199254740991, required: !1
                        }
                        , min: {
                            type: Number, default: Number.MIN_SAFE_INTEGER || -9007199254740991, required: !1
                        }
                        , minus: {
                            type: Boolean, default: 1, required: !1
                        }
                        , placeholder: {
                            type: String, default: "", required: !1
                        }
                        , emptyValue: {
                            type: [Number, String], default: "", required: !1
                        }
                        , precision: {
                            type: Number, default: 0, required: !1
                        }
                        , separator: {
                            type: String, default: ",", required: !1
                        }
                        , thousandSeparator: {
                            default: void 0, required: !1, type: String
                        }
                        , decimalSeparator: {
                            default: void 0, required: !1, type: String
                        }
                        , outputType: {
                            required: !1, type: String, default: "Number"
                        }
                        , value: {
                            type: [Number, String], default: 0, required: !0
                        }
                        , readOnly: {
                            type: Boolean, default: !1, required: !1
                        }
                        , readOnlyClass: {
                            type: String, default: "", required: !1
                        }
                        , currencySymbolPosition: {
                            type: String, default: "prefix", required: !1
                        }
                    }
                    , data: function () {
                        return {
                            amount: ""
                        }
                    }
                    , computed: {
                        amountNumber: function () {
                            return this.unformat(this.amount)
                        }
                        , valueNumber: function () {
                            return this.unformat(this.value)
                        }
                        , decimalSeparatorSymbol: function () {
                            return void 0 !== this.decimalSeparator ? this.decimalSeparator : "," === this.separator ? "." : ","
                        }
                        , thousandSeparatorSymbol: function () {
                            return void 0 !== this.thousandSeparator ? this.thousandSeparator : "." === this.separator ? "." : "space" === this.separator ? " " : ","
                        }
                        , symbolPosition: function () {
                            return this.currency ? "suffix" === this.currencySymbolPosition ? "%v %s" : "%s %v" : "%v"
                        }
                    }
                    , watch: {
                        valueNumber: function (e) {
                            this.$refs.numeric !== document.activeElement && (this.amount = this.format(e))
                        }
                        , readOnly: function (e, t) {
                            var r = this;
                            !1 === t && !0 === e && this.$nextTick(function () {
                                r.$refs.readOnly.className = r.readOnlyClass
                            }
                            )
                        }
                        , separator: function () {
                            this.process(this.valueNumber), this.amount = this.format(this.valueNumber)
                        }
                        , currency: function () {
                            this.process(this.valueNumber), this.amount = this.format(this.valueNumber)
                        }
                        , precision: function () {
                            this.process(this.valueNumber), this.amount = this.format(this.valueNumber)
                        }
                    }
                    , mounted: function () {
                        var e = this;
                        this.placeholder || (this.process(this.valueNumber), this.amount = this.format(this.valueNumber), setTimeout(function () {
                            e.process(e.valueNumber), e.amount = e.format(e.valueNumber)
                        }
                            , 500)), this.readOnly && (this.$refs.readOnly.className = this.readOnlyClass)
                    }
                    , methods: {
                        addCommas: function (nStr) {
                            if (typeof (nStr) === "undefined")
                                return "";

                            nStr += '';
                            var x = nStr.split('.');
                            var x1 = x[0];
                            var x2 = x.length > 1 ? '.' + x[1] : '';
                            var rgx = /(\d+)(\d{3})/;
                            while (rgx.test(x1)) {
                                x1 = x1.replace(rgx, '$1' + ',' + '$2');
                            }

                            return x1 + x2;
                        },
                        onChangeHandler: function (e) {
                            this.$emit("change", e);
                        },
                        onBlurHandler: function (e) {
                            this.$emit("blur", e), this.amount = this.format(this.valueNumber)
                        }
                        , onFocusHandler: function (e) {
                            this.$emit("focus", e), 0 === this.valueNumber ? this.amount = null : this.amount = this.valueNumber;

                            e.target.selectionStart = 0;
                            e.target.selectionEnd = 0;
                        }
                        , onInputHandler: function () {
                            this.process(this.amountNumber)
                        }
                        , process: function (e) {
                            e >= this.max && this.update(this.max), e <= this.min && this.update(this.min), e > this.min && e < this.max && this.update(e), !this.minus && e < 0 && (this.min >= 0 ? this.update(this.min) : this.update(0))
                        }
                        , update: function (e) {
                            var t = Number(e).toFixed(this.precision), r = "string" === this.outputType.toLowerCase() ? t : Number(t);
                            this.$emit("input", r);
                        }
                        , format: function (e) {
                            return this.addCommas(e);
                        }
                        , unformat: function (e) {
                            var t = "string" == typeof e && "" === e ? this.emptyValue : e;
                            return t;
                        }
                    }
                }
            }
                , function (e, t, r) {
                    "use strict";
                    Object.defineProperty(t, "__esModule", {
                        value: !0
                    }
                    );
                    var n = r(2), i = {
                        install: function (e) {
                            e.component(n.a.name, n.a)
                        }
                    }
                        ;
                    n.a.install = i.install, t.default = n.a
                }
                , function (e, t, r) {
                    "use strict";
                    var n = r(0), i = r(5), u = r(3), o = u(n.a, i.a, !1, null, null, null);
                    t.a = o.exports
                }
                , function (e, t) {
                    e.exports = function (e, t, r, n, i, u) {
                        var o, a = e = e || {}
                            , s = typeof e.default;
                        "object" !== s && "function" !== s || (o = e, a = e.default);
                        var c = "function" == typeof a ? a.options : a;
                        t && (c.render = t.render, c.staticRenderFns = t.staticRenderFns, c._compiled = !0), r && (c.functional = !0), i && (c._scopeId = i);
                        var l;
                        if (u ? (l = function (e) {
                            e = e || this.$vnode && this.$vnode.ssrContext || this.parent && this.parent.$vnode && this.parent.$vnode.ssrContext, e || "undefined" == typeof __VUE_SSR_CONTEXT__ || (e = __VUE_SSR_CONTEXT__), n && n.call(this, e), e && e._registeredComponents && e._registeredComponents.add(u)
                        }
                            , c._ssrRegister = l) : n && (l = n), l) {
                            var m = c.functional, d = m ? c.render : c.beforeCreate;
                            m ? (c._injectStyles = l, c.render = function (e, t) {
                                return l.call(t), d(e, t)
                            }
                            ) : c.beforeCreate = d ? [].concat(d, l) : [l]
                        }
                        return {
                            esModule: o, exports: a, options: c
                        }
                    }
                }
                , function (t, r) {
                    t.exports = e
                }
                , function (e, t, r) {
                    "use strict";
                    var n = function () {
                        var e = this, t = e.$createElement, r = e._self._c || t;
                        return e.readOnly ? r("span", {
                            ref: "readOnly"
                        }
                            , [e._v(e._s(e.amount))]) : r("input", {
                                directives: [{
                                    name: "model", rawName: "v-model", value: e.amount, expression: "amount"
                                }
                                ], ref: "numeric", attrs: {
                                    placeholder: e.placeholder, type: "text", class: "form-control text-right"
                                }
                                , domProps: {
                                    value: e.amount
                                }
                                , on: {
                                    blur: e.onBlurHandler,
                                    input: [function (t) {
                                        t.target.composing || (e.amount = t.target.value)
                                    }, e.onInputHandler],
                                    focus: e.onFocusHandler,
                                    change: e.onChangeHandler
                                }
                            }
                            )
                    }
                        , i = [], u = {
                            render: n, staticRenderFns: i
                        }
                        ;
                    t.a = u
                }
            ])
    }

    );