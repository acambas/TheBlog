

window.blogNamespace.editPostContext = (function (baseContext, $, Select2) {
    //Private
    var tags;

    //Constructor
    var editPostContext = {
        init: init
    }
    return editPostContext;

    //Public
    function init(tagsData, formId, tagsFieldId, deleteFieldId, postId, deleteUrl, postListUrl) {
        this.tags = tagsData;

        $(deleteFieldId).click(function () {
            var conf = confirm("Are you sure you want to delete this post");

            if (conf == true) {
                baseContext.basicSpinner();
                $.post(deleteUrl, { id: postId }).done(function () {
                       
                    location.href = postListUrl;
                });
            }
        });

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

})(blogNamespace.baseContext, $, Select2)