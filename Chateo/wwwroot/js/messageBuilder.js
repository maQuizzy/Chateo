var CreateOtherMessage = function(text, hour, min) {
    var descBlock = CreateMessageDescBlock("chat__message-desc", hour, min);
    var textParagraph = CreateTextParagraph(text);
    var messageContent = CreateMessageContentBlock("chat__other-message-content", textParagraph, descBlock);
    var messageBlock = CreateMessageBlock("chat__other-message", messageContent);

    return messageBlock;
}

var CreateSelfMessage = function(text, hour, min) {
    var descBlock = CreateMessageDescBlock("chat__message-desc", hour, min);
    var textParagraph = CreateTextParagraph(text);
    var messageContent = CreateMessageContentBlock("chat__self-message-content", textParagraph, descBlock);
    var messageBlock = CreateMessageBlock("chat__self-message", messageContent);

    return messageBlock;
}

var CreateMessageBlock = function(className, messageContentBlock) {
    var messageBlock = document.createElement("div");

    messageBlock.classList.add(className);

    messageBlock.appendChild(messageContentBlock);

    return messageBlock;
}

var CreateMessageContentBlock = function(
    className,
    textParagraph,
    messageDescBlock) {
    var messageContentBlock = document.createElement("div");

    messageContentBlock.classList.add(className);
    messageContentBlock.appendChild(textParagraph);
    messageContentBlock.appendChild(messageDescBlock);

    return messageContentBlock;
}

var CreateTextParagraph = function(messageText) {
    var textParagraph = document.createElement("p");
    textParagraph.textContent = messageText;

    return textParagraph;
}

var CreateMessageDescBlock = function(classname, hour, min) {
    var messageDescBlock = document.createElement("div");

    messageDescBlock.classList.add(classname);

    var timeParagraph = document.createElement("p");
    timeParagraph.textContent = hour + ':' + min;

    var dotParagraph = document.createElement("p");
    dotParagraph.textContent = "·";

    var statusParagraph = document.createElement("p");
    statusParagraph.textContent = "Read";

    messageDescBlock.appendChild(timeParagraph);
    messageDescBlock.appendChild(dotParagraph);
    messageDescBlock.appendChild(statusParagraph);

    return messageDescBlock;
}