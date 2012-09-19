(function () {
    var mappingProperty = "__ko_mapping__";
    ko.mapping.toJS = function (rootObject, options) {
        if (arguments.length == 0) throw new Error("When calling ko.mapping.toJS, pass the object you want to convert.");

        options = options || {};
        options.ignore = options.ignore || [];
        if (!(options.ignore instanceof Array)) {
            options.ignore = [options.ignore];
        }

        options.ignore.push(mappingProperty);

        // We just unwrap everything at every level in the object graph
        return ko.mapping.visitModel(rootObject, function (x) {
            var res = ko.utils.unwrapObservable(x);
            if (Object.prototype.toString.call(res) === '[object Date]')
                res = '\\/Date(' + res.getTime() + ')\\/';
            return res;
        }, options);
    };
})();

ko.bindingHandlers.valueExt = {
    'init': function (element, valueAccessor, allBindingsAccessor) {
        var eventName = allBindingsAccessor()["valueUpdate"] || "change";
        var valueType = allBindingsAccessor()["valueType"] || MvcControlsToolkit_DataType_String;
        var elementType = $(element).attr("element-type") || "";
        if (eventName == "change" && element.tagName != "SELECT" && element.tagName != "CHECKBOX")
            eventName = "blur";
        if (elementType != "") eventName = elementType + "_Changed";

        // The syntax "after<eventname>" means "run the handler asynchronously after the event"
        // This is useful, for example, to catch "keydown" events after the browser has updated the control
        // (otherwise, ko.selectExtensions.readValue(this) will receive the control's value *before* the key event)
        var handleEventAsynchronously = false;
        if (elementType == "" && eventName.length > 7 && ko.utils.stringStartsWith(eventName, "after")) {
            handleEventAsynchronously = true;
            eventName = eventName.substring("after".length);
        }
        var runEventHandler = handleEventAsynchronously ? function (handler) { setTimeout(handler, 0) }
                                                        : function (handler) { handler() };

        ko.utils.registerEventHandler(element, eventName, function () {
            runEventHandler(function () {
                var modelValue = valueAccessor();
                var elementValue = null;
                if (elementType != "") {
                    elementValue = eval("MvcControlsToolkit_" + elementType + "_Get(element, valueType)");
                }
                else {
                    elementValue = MvcControlsToolkit_Parse(
                    ko.selectExtensions.readValue(element),
                    valueType);
                }
                if (isNaN(elementValue)) elementValue = ko.selectExtensions.readValue(element);
                if (ko.isWriteableObservable(modelValue))
                    modelValue(elementValue);
                else {
                    var allBindings = allBindingsAccessor();
                    if (allBindings['_ko_property_writers'] && allBindings['_ko_property_writers']['value'])
                        allBindings['_ko_property_writers']['value'](elementValue);
                }
            });
        });
    },
    'update': function (element, valueAccessor, allBindingsAccessor) {
        var valueType = allBindingsAccessor()["valueType"] || MvcControlsToolkit_DataType_String;
        var formatString = allBindingsAccessor()["formatString"] || '';
        var elementType = $(element).attr("element-type") || "";
        if (elementType != "") eventName = elementType + "_changed";
        
        var newValue = ko.utils.unwrapObservable(valueAccessor());
        if (valueType == 4 && newValue != null) {

        }
        var elementValue = null;
        if (elementType != "") {
            elementValue = eval("MvcControlsToolkit_" + elementType + "_Get(element, valueType)");
        }
        else {
            elementValue = MvcControlsToolkit_Parse(
                    ko.selectExtensions.readValue(element),
                    valueType);
        }
        if (isNaN(elementValue)) elementValue = ko.selectExtensions.readValue(element);
        var valueHasChanged = (newValue != elementValue);
        // JavaScript's 0 == "" behavious is unfortunate here as it prevents writing 0 to an empty text box (loose equality suggests the values are the same). 
        // We don't want to do a strict equality comparison as that is more confusing for developers in certain cases, so we specifically special case 0 != "" here.
        if ((newValue === 0) && (elementValue !== 0) && (elementValue !== "0"))
            valueHasChanged = true;

        if (valueHasChanged) {
            var convertedValue = null;
            var applyValueAction = null;
            if (elementType != "") {
                applyValueAction = function () {
                    eval("MvcControlsToolkit_" + elementType + "_Set(element, newValue, formatString, valueType);");
                    
                };
            }
            else {
                convertedValue = MvcControlsToolkit_ToString(newValue, formatString, valueType);
                applyValueAction = function () {
                    ko.selectExtensions.writeValue(element, convertedValue);
                    ko.utils.triggerEvent(element, "blur");
                };
            }
            applyValueAction();

            // Workaround for IE6 bug: It won't reliably apply values to SELECT nodes during the same execution thread
            // right after you've changed the set of OPTION nodes on it. So for that node type, we'll schedule a second thread
            // to apply the value as well.
            var alsoApplyAsynchronously = element.tagName == "SELECT";
            if (alsoApplyAsynchronously)
                setTimeout(applyValueAction, 0);
        }

        // For SELECT nodes, you're not allowed to have a model value that disagrees with the UI selection, so if there is a
        // difference, treat it as a change that should be written back to the model

        if (element.tagName == "SELECT") {
            if (elementType != "") {
                elementValue = eval("MvcControlsToolkit_" + elementType + "_Get(element, valueType)");
            }
            else {
                elementValue = MvcControlsToolkit_Parse(
                    ko.selectExtensions.readValue(element),
                    valueType);
            }
            if (elementValue !== newValue)
                ko.utils.triggerEvent(element, "change");
        }

    }
};
ko.jqueryTmplTemplateEngineExt = function () {
    // Detect which version of jquery-tmpl you're using. Unfortunately jquery-tmpl 
    // doesn't expose a version number, so we have to infer it.
    this.jQueryTmplVersion = (function () {
        if ((typeof (jQuery) == "undefined") || !jQuery['tmpl'])
            return 0;
        // Since it exposes no official version number, we use our own numbering system. To be updated as jquery-tmpl evolves.
        if (jQuery['tmpl']['tag']) {
            if (jQuery['tmpl']['tag']['tmpl'] && jQuery['tmpl']['tag']['tmpl']['open']) {
                if (jQuery['tmpl']['tag']['tmpl']['open'].toString().indexOf('__') >= 0) {
                    return 3; // Since 1.0.0pre, custom tags should append markup to an array called "__"
                }
            }
            return 2; // Prior to 1.0.0pre, custom tags should append markup to an array called "_"
        }
        return 1; // Very old version doesn't have an extensible tag system
    })();

    function getTemplateNode(template) {
        var templateNode = document.getElementById(template);
        if (templateNode == null)
            throw new Error("Cannot find template with ID=" + template);
        return templateNode;
    }

    // These two only needed for jquery-tmpl v1
    var aposMarker = "__ko_apos__";
    var aposRegex = new RegExp(aposMarker, "g");

    this['renderTemplate'] = function (templateId, data, options) {
        options = options || {};
        if (this.jQueryTmplVersion == 0)
            throw new Error("jquery.tmpl not detected.\nTo use KO's default template engine, reference jQuery and jquery.tmpl. See Knockout installation documentation for more details.");

        if (this.jQueryTmplVersion == 1) {
            // jquery.tmpl v1 doesn't like it if the template returns just text content or nothing - it only likes you to return DOM nodes.
            // To make things more flexible, we can wrap the whole template in a <script> node so that jquery.tmpl just processes it as
            // text and doesn't try to parse the output. Then, since jquery.tmpl has jQuery as a dependency anyway, we can use jQuery to
            // parse that text into a document fragment using jQuery.clean().        
            var templateTextInWrapper = "<script type=\"text/html\">" + getTemplateNode(templateId).text + "</script>";
            var renderedMarkupInWrapper = jQuery['tmpl'](templateTextInWrapper, data);
            var renderedMarkup = renderedMarkupInWrapper[0].text.replace(aposRegex, "'"); ;
            var finalRes = jQuery['clean']([renderedMarkup], document);
            //renderedMarkup = finalRes.text();
            //finalRes = jQuery['clean']([renderedMarkup], document);
            return finalRes;
        }

        // It's easier with jquery.tmpl v2 and later - it handles any DOM structure
        if (!(templateId in jQuery['template'])) {
            // Precache a precompiled version of this template (don't want to reparse on every render)
            var templateText = getTemplateNode(templateId).text;
            jQuery['template'](templateId, templateText);
        }
        data = [data]; // Prewrap the data in an array to stop jquery.tmpl from trying to unwrap any arrays

        var resultNodes = jQuery['tmpl'](templateId, data, options['templateOptions']);
        resultNodes['appendTo'](document.createElement("div")); // Using "appendTo" forces jQuery/jQuery.tmpl to perform necessary cleanup work
        //var renderedMarkup = resultNodes.text();
        //resultNodes = jQuery['clean']([renderedMarkup], document);
        jQuery['fragments'] = {}; // Clear jQuery's fragment cache to avoid a memory leak after a large number of template renders

        return resultNodes;
    },

    this['isTemplateRewritten'] = function (templateId) {
        // It must already be rewritten if we've already got a cached version of it
        // (this optimisation helps on IE < 9, because it greatly reduces the number of getElementById calls)
        if (templateId in jQuery['template'])
            return true;

        return getTemplateNode(templateId).isRewritten === true;
    },

    this['rewriteTemplate'] = function (template, rewriterCallback) {
        var templateNode = getTemplateNode(template);
        var rewritten = rewriterCallback($('<div/>').html(templateNode.text).text());

        if (this.jQueryTmplVersion == 1) {
            // jquery.tmpl v1 falls over if you use single-quotes, so replace these with a temporary marker for template rendering, 
            // and then replace back after the template was rendered. This is slightly complicated by the fact that we must not interfere
            // with any code blocks - only replace apos characters outside code blocks.
            rewritten = ko.utils.stringTrim(rewritten);
            rewritten = rewritten.replace(/([\s\S]*?)(\${[\s\S]*?}|{{[\=a-z][\s\S]*?}}|$)/g, function (match) {
                // Called for each non-code-block followed by a code block (or end of template)
                var nonCodeSnippet = arguments[1];
                var codeSnippet = arguments[2];
                return nonCodeSnippet.replace(/\'/g, aposMarker) + codeSnippet;
            });
        }

        templateNode.text = '${MvcControlsToolkit_NewTemplateName($item) } ' + rewritten;
        templateNode.isRewritten = true;
    },

    this['createJavaScriptEvaluatorBlock'] = function (script) {
        if (this.jQueryTmplVersion == 1)
            return "{{= " + script + "}}";

        // From v2, jquery-tmpl does some parameter parsing that fails on nontrivial expressions.
        // Prevent it from messing with the code by wrapping it in a further function.
        return "{{ko_code ((function() { return " + script + " })()) }}";
    },

    this.addTemplate = function (templateName, templateMarkup) {
        document.write("<script type='text/html' id='" + templateName + "'>" + templateMarkup + "</script>");
    }
    ko.exportProperty(this, 'addTemplate', this.addTemplate);

    if (this.jQueryTmplVersion > 1) {
        jQuery['tmpl']['tag']['ko_code'] = {
            open: (this.jQueryTmplVersion < 3 ? "_" : "__") + ".push($1 || '');"
        };
    }
};

ko.jqueryTmplTemplateEngineExt.prototype = new ko.templateEngine();

// Use this one by default
ko.setTemplateEngine(new ko.jqueryTmplTemplateEngineExt());

ko.exportSymbol('ko.jqueryTmplTemplateEngineExt', ko.jqueryTmplTemplateEngineExt);

