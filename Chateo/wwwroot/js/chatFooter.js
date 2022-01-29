$('#messageFileInput').change(function (e) {
    var file = e.target.files[0];
    var reader = new FileReader();
    reader.onload = function (e) {
        var image = document.createElement("img");
        image.src = e.target.result;

        var imagePreview = document.getElementById("footerImagePreview");

        var remainingImage = imagePreview.querySelector("img");
        if (remainingImage != null)
            remainingImage.remove();

        imagePreview.append(image);
        imagePreview.style.display = "block";
    }
    reader.readAsDataURL(file);
});