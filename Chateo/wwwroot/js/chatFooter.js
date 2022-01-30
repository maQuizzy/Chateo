$('#messageFileInput').change(function (e) {
    var file = e.target.files[0];
    var reader = new FileReader();
    reader.onload = function (e) {
        var image = document.createElement("img");
        image.src = e.target.result;

        var footerPreview = document.getElementById("footerPreview");

        var remainingImage = footerPreview.querySelector("img");
        if (remainingImage != null)
            remainingImage.remove();

        footerPreview.append(image);
        footerPreview.style.display = "block";
    }
    reader.readAsDataURL(file);
});

let setFooterReplyState = (display) => {

    var footerPreview = document.getElementById("footerPreview");
    var remainingImage = footerPreview.querySelector("img");

    if (display) {
        $(".footer__preview").css("display", "block");
    }
    else if (remainingImage == null) {
        $(".footer__preview").css("display", "none");
    }
}

let addMessageToFooterPreview = (username, messageText) =>
{
    $('.footer__replied-from').text(username);
    $('.footer__replied-text').text(messageText);
    $('.footer__replied-message').css("display", "block");
}

let deleteMessageFromFooterPreview = () => {
    $('.footer__replied-from').text('');
    $('.footer__replied-text').text('');
    $('.footer__replied-message').css("display", "none");
}