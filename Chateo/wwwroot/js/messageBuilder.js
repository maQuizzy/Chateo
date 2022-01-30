var CreateOtherMessage = function (id, username, text, image, time, repliedUsername, repliedText, repliedImage) {
    var descBlock = CreateMessageDescBlock("chat__message-desc", time);
    var textParagraph = CreateTextParagraph(text);
    var messageContent = CreateMessageContentBlock("chat__other-message-content",
        textParagraph,
        descBlock,
        image,
        repliedUsername,
        repliedText,
        repliedImage,
        "other-replied-message");
    var messageBlock = CreateMessageBlock("chat__other-message", messageContent, id, username);
    var checkImage = CreateCheckImage();

    checkImage.classList.add('check-other-message');
    messageBlock.append(checkImage);

    messageBlock.id = "message-" + id;

    return messageBlock;
}

var CreateSelfMessage = function (id, username, text, image, time, repliedUsername, repliedText, repliedImage) {
    var descBlock = CreateMessageDescBlock("chat__message-desc", time);
    var textParagraph = CreateTextParagraph(text);
    var messageContent = CreateMessageContentBlock("chat__self-message-content",
        textParagraph,
        descBlock,
        image,
        repliedUsername,
        repliedText,
        repliedImage,
        "self-replied-message");
    var messageBlock = CreateMessageBlock("chat__self-message", messageContent, id, username);
    var checkImage = CreateCheckImage();

    checkImage.classList.add('check-self-message');
    messageBlock.prepend(checkImage);


    messageBlock.id = "message-" + id;

    return messageBlock;
}

var CreateCheckImage = function () {
    var checkImg = document.createElement("img");
    checkImg.src = "/img/check.svg";
    checkImg.classList.add("chat__check-message")

    return checkImg;
}

var CreateMessageBlock = function (className, messageContentBlock, id, username) {
    var messageBlock = document.createElement("div");

    messageBlock.classList.add(className);

    messageBlock.appendChild(messageContentBlock);

    var messageUsernameBlock = document.createElement("p");
    messageUsernameBlock.classList.add("chat__message-username");
    messageUsernameBlock.style = "display:none";
    messageUsernameBlock.textContent = username;

    var messageIdBlock = document.createElement("p");
    messageIdBlock.classList.add("chat__message-id");
    messageIdBlock.style = "display:none";
    messageIdBlock.textContent = id;

    messageBlock.appendChild(messageUsernameBlock);
    messageBlock.appendChild(messageIdBlock);

    return messageBlock;
}

var CreateMessageContentBlock = function (
    className,
    textParagraph,
    messageDescBlock,
    image,
    repliedUsername,
    repliedText,
    repliedImage,
    repliedClass) {
    var messageContentBlock = document.createElement("div");

    messageContentBlock.classList.add(className);

    if (repliedUsername != null) {
        var repliedMessageBlock = document.createElement("div");
        repliedMessageBlock.classList.add("chat__replied-message", repliedClass);

        var repliedFromBlock = document.createElement("p");
        repliedFromBlock.classList.add("chat__replied-from");
        repliedFromBlock.textContent = repliedUsername;

        var repliedTextBlock = document.createElement("p");
        repliedTextBlock.classList.add("chat__replied-text");
        repliedTextBlock.textContent = repliedText;

        if (repliedClass == "other-replied-message") {
            repliedFromBlock.classList.add("other-replied-from");
            repliedTextBlock.classList.add("other-replied-text");
        }
        else {
            repliedFromBlock.classList.add("self-replied-from");
            repliedTextBlock.classList.add("self-replied-text");
        }

        repliedMessageBlock.appendChild(repliedFromBlock);
        repliedMessageBlock.appendChild(repliedTextBlock);

        if (repliedImage != null) {
            let repliedImageBlock = document.createElement("img");
            repliedImageBlock.src = `data:image/jpeg;base64,${repliedImage}`;
            repliedImageBlock.classList.add("chat__message-img");

            repliedMessageBlock.appendChild(repliedImageBlock);
        }

        messageContentBlock.appendChild(repliedMessageBlock);
    }

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
    textParagraph.classList.add("chat__message-text");
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