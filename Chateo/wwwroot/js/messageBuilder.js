var CreateOtherMessage = function (id, text, image, time) {
    var descBlock = CreateMessageDescBlock("chat__message-desc", time);
    var textParagraph = CreateTextParagraph(text);
    var messageContent = CreateMessageContentBlock("chat__other-message-content", textParagraph, descBlock, image);
    var messageBlock = CreateMessageBlock("chat__other-message", messageContent);

    messageBlock.id = "message-" + id;

    return messageBlock;
}

var CreateSelfMessage = function (id, text, image, time) {
    var descBlock = CreateMessageDescBlock("chat__message-desc", time);
    var textParagraph = CreateTextParagraph(text);
    var messageContent = CreateMessageContentBlock("chat__self-message-content", textParagraph, descBlock, image);
    var messageBlock = CreateMessageBlock("chat__self-message", messageContent);

    messageBlock.id = "message-" + id;

    return messageBlock;
}

var CreateMessageBlock = function (className, messageContentBlock) {
    var messageBlock = document.createElement("div");

    messageBlock.classList.add(className);

    messageBlock.appendChild(messageContentBlock);

    return messageBlock;
}

var CreateMessageContentBlock = function (
    className,
    textParagraph,
    messageDescBlock,
    image) {
    var messageContentBlock = document.createElement("div");

    messageContentBlock.classList.add(className);

    if (image != null) {
        var imageBlock = document.createElement("img");
        imageBlock.classList.add("chat__message-img");
        imageBlock.src = `data:image/jpeg;base64,${image}`;
        messageContentBlock.appendChild(imageBlock);
    }

    messageContentBlock.appendChild(textParagraph);
    messageContentBlock.appendChild(messageDescBlock);

    return messageContentBlock;
}

var CreateTextParagraph = function (messageText) {
    var textParagraph = document.createElement("p");
    textParagraph.textContent = messageText;

    return textParagraph;
}

var CreateMessageDescBlock = function (classname, time) {
    var messageDescBlock = document.createElement("div");

    messageDescBlock.classList.add(classname);

    var timeParagraph = document.createElement("p");
    timeParagraph.textContent = time;

    messageDescBlock.appendChild(timeParagraph);

    return messageDescBlock;
}