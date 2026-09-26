function showStandardLoading(message) {
    var m = document.getElementById('standardLoadingMessage');
    if (m) m.textContent = message || 'Loading...';
    var o = document.getElementById('standardLoadingOverlay');
    if (o) o.classList.add('show');
}

function hideStandardLoading() {
    var o = document.getElementById('standardLoadingOverlay');
    if (o) o.classList.remove('show');
}

function initVerifyTabs() {
    var buttons = document.querySelectorAll('.verify-tab');
    Array.prototype.forEach.call(buttons, function (btn) {
        btn.addEventListener('click', function () {
            var root = btn.closest('.verify-page-card') || document;
            Array.prototype.forEach.call(root.querySelectorAll('.verify-tab'), function (x) { x.classList.remove('active'); });
            Array.prototype.forEach.call(root.querySelectorAll('.verify-tab-panel'), function (x) { x.classList.remove('active'); });
            btn.classList.add('active');
            var panel = document.querySelector(btn.getAttribute('data-target'));
            if (panel) panel.classList.add('active');

            var jq = window.jQuery;
            if (jq && jq.fn && jq.fn.dataTable) {
                jq.fn.dataTable.tables({ visible: true, api: true }).columns.adjust();
            }
        });
    });
}

function initVerifyDataTable(selector) {
    var jq = window.jQuery;
    if (!jq || !jq.fn || !jq.fn.DataTable || !jq(selector).length) return;
    if (jq.fn.DataTable.isDataTable(selector)) return;

    jq(selector).DataTable({
        pageLength: 25,
        lengthMenu: [[10, 25, 50, 100, -1], [10, 25, 50, 100, 'All']],
        autoWidth: false,
        scrollX: true,
        order: [],
        language: {
            search: 'Search:',
            emptyTable: 'No records available'
        }
    });
}

function showVerifyMessage(message, isError) {
    var el = document.getElementById('dvError');
    if (!el) return;
    el.className = 'verify-alert ' + (isError ? 'error' : 'success');
    el.textContent = message;
    el.style.display = 'block';
    window.setTimeout(function () { el.style.display = 'none'; }, 5000);
}

function loadScriptOnce(src, id, done, failed) {
    if (id && document.getElementById(id)) {
        if (done) done();
        return;
    }
    var s = document.createElement('script');
    if (id) s.id = id;
    s.src = src;
    s.onload = function () { if (done) done(); };
    s.onerror = function () { if (failed) failed(); };
    document.head.appendChild(s);
}

function ensureDataTables(done) {
    function loadDataTables() {
        var jq = window.jQuery;
        if (!jq) {
            if (done) done(false);
            return;
        }
        if (jq.fn && jq.fn.DataTable) {
            if (done) done(true);
            return;
        }
        loadScriptOnce(
            'https://cdn.datatables.net/1.13.8/js/jquery.dataTables.min.js',
            'verifyDataTablesJs',
            function () { if (done) done(!!(window.jQuery && window.jQuery.fn && window.jQuery.fn.DataTable)); },
            function () { if (done) done(false); }
        );
    }

    if (window.jQuery) {
        loadDataTables();
        return;
    }

    loadScriptOnce(
        'https://code.jquery.com/jquery-3.7.1.min.js',
        'verifyJQueryJs',
        loadDataTables,
        function () { if (done) done(false); }
    );
}

function pageMethod(url, payload, onSuccess) {
    payload = payload || {};
    var loadingMessage = payload.loadingMessage || 'Processing...';
    if (Object.prototype.hasOwnProperty.call(payload, 'loadingMessage')) delete payload.loadingMessage;
    showStandardLoading(loadingMessage);

    if (window.fetch) {
        fetch(url, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json; charset=utf-8' },
            credentials: 'same-origin',
            body: JSON.stringify(payload)
        })
        .then(function (response) {
            return response.text().then(function (text) {
                var data = null;
                try { data = text ? JSON.parse(text) : {}; } catch (e) { }
                if (!response.ok) {
                    var message = data && (data.Message || data.message) ? (data.Message || data.message) : ('Request failed (' + response.status + ').');
                    throw new Error(message);
                }
                return data;
            });
        })
        .then(function (r) {
            hideStandardLoading();
            if (onSuccess) onSuccess(r && Object.prototype.hasOwnProperty.call(r, 'd') ? r.d : r);
        })
        .catch(function (err) {
            hideStandardLoading();
            showVerifyMessage(err.message || 'Request failed.', true);
        });
        return;
    }

    var jq = window.jQuery;
    if (!jq || !jq.ajax) {
        hideStandardLoading();
        showVerifyMessage('Unable to process request because JavaScript libraries are unavailable.', true);
        return;
    }

    jq.ajax({
        type: 'POST',
        url: url,
        data: JSON.stringify(payload),
        contentType: 'application/json; charset=utf-8',
        dataType: 'json'
    }).done(function (r) {
        hideStandardLoading();
        if (onSuccess) onSuccess(r.d);
    }).fail(function (xhr) {
        hideStandardLoading();
        var msg = 'Request failed.';
        try { msg = JSON.parse(xhr.responseText).Message || msg; } catch (e) { }
        showVerifyMessage(msg, true);
    });
}

window.addEventListener('load', function () {
    window.setTimeout(hideStandardLoading, 100);
});
