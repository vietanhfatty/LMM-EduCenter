"use strict";

var NotificationHub = (function () {
    var connection = null;
    var unreadCount = 0;

    function init(serverUrl, accessToken) {
        if (!accessToken) return;

        connection = new signalR.HubConnectionBuilder()
            .withUrl(serverUrl + "/hubs/notification", {
                accessTokenFactory: function () { return accessToken; }
            })
            .withAutomaticReconnect([0, 2000, 5000, 10000, 30000])
            .configureLogging(signalR.LogLevel.Warning)
            .build();

        connection.on("ReceiveNotification", function (notification) {
            unreadCount++;
            updateBadge();
            showToast(notification.title, notification.content);
        });

        connection.on("NotificationRead", function (notificationId) {
            if (unreadCount > 0) unreadCount--;
            updateBadge();
        });

        connection.onreconnecting(function () {
            console.log("[SignalR] Reconnecting...");
        });

        connection.onreconnected(function () {
            console.log("[SignalR] Reconnected.");
        });

        connection.onclose(function () {
            console.log("[SignalR] Disconnected.");
        });

        connection.start()
            .then(function () {
                console.log("[SignalR] Connected to NotificationHub.");
            })
            .catch(function (err) {
                console.error("[SignalR] Connection failed:", err.toString());
            });
    }

    function updateBadge() {
        var badge = document.getElementById("notification-badge");
        if (!badge) return;

        if (unreadCount > 0) {
            badge.textContent = unreadCount > 99 ? "99+" : unreadCount;
            badge.style.display = "inline-block";
        } else {
            badge.style.display = "none";
        }
    }

    function setUnreadCount(count) {
        unreadCount = count;
        updateBadge();
    }

    function showToast(title, content) {
        var container = document.getElementById("notification-toast-container");
        if (!container) {
            container = document.createElement("div");
            container.id = "notification-toast-container";
            container.className = "toast-container position-fixed top-0 end-0 p-3";
            container.style.zIndex = "1080";
            document.body.appendChild(container);
        }

        var toastId = "toast-" + Date.now();
        var toastHtml =
            '<div id="' + toastId + '" class="toast show border-0 shadow" role="alert">' +
            '  <div class="toast-header bg-primary text-white">' +
            '    <strong class="me-auto"><i class="bi bi-bell-fill me-1"></i>' + escapeHtml(title) + '</strong>' +
            '    <small>Vua xong</small>' +
            '    <button type="button" class="btn-close btn-close-white" data-bs-dismiss="toast"></button>' +
            '  </div>' +
            '  <div class="toast-body">' + escapeHtml(content) + '</div>' +
            '</div>';

        container.insertAdjacentHTML("beforeend", toastHtml);

        var toastEl = document.getElementById(toastId);
        setTimeout(function () {
            if (toastEl) toastEl.remove();
        }, 5000);
    }

    function escapeHtml(text) {
        var div = document.createElement("div");
        div.appendChild(document.createTextNode(text));
        return div.innerHTML;
    }

    return {
        init: init,
        setUnreadCount: setUnreadCount
    };
})();
