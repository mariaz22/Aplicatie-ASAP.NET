

$(document).ready(function () {
    $('.summernote').summernote({
        height: 300,
        minHeight: 200,
        focus: true,
        toolbar: [
            ['insert', ['picture', 'video']]
        ],
        callbacks: {
            onInit: function () {
                checkImageExistence();
            },
            onChange: function () {
                checkImageExistence();
            },
            onPaste: function () {
                setTimeout(function () {
                    checkImageExistence();
                }, 100);
            }
        }
    });

    function checkImageExistence() {
        var content = $('.summernote').summernote('code');
        var containsMedia = $(content).find('img').length > 0 || $(content).find('video').length > 0;

        if (containsMedia) {
            imageAdded = true;
        }
    }
});
