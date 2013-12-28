

window.blogNamespace.createPostContext = (function (baseContext, $,Select2) {
    //Private
    var tags;
    
    //Constructor
    var createPostContext = {
        init: init
    }
    return createPostContext;

    //Public
    function init(tagsData, formId, tagsFieldId) {
        this.tags = tagsData;

        $(formId).submit(function (event) {
            if (!$(this).valid()) {
                return false;
            }

            baseContext.basicSpinner();
        });
      

        $(tagsFieldId).select2({
            tags: this.tags,
            maximumInputLength: 15,
            tokenSeparators: [",", " "]
        });
    }

})(blogNamespace.baseContext, $,Select2)