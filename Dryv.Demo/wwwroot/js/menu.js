
$(document).ready(function () {
    var html = "";
    var lastSize = 0;
    var size = 0;
    $("h1,h2,h3", $(".content-inner")).each(function (i, e) {
        var $e = $(e);
        var caption = $e.text().trim().replace(/\s{2,}/g, " ");
        if (caption) {
            size = $e.prop("tagName").substr(1, 1);
            var id = $e.attr("id");
            if (lastSize) {
                if (size > lastSize) {
                    html += '<ul class="nav doc-sub-menu">';
                }

                for (var i = size; i < lastSize; i++) {
                    html += '</ul>';
                }
            }
            if (lastSize) {
                html += '</li>';
            }
            html += '<li><a class="scrollto" href="#' + id + '">' + caption + '</a>';
            lastSize = size;
        }
    });

    if (lastSize) {
        for (var i = size; i < lastSize; i++) {
            html += '</ul>';
        }
        html += '</li>';
    }

    $("#doc-menu").html(html);
});