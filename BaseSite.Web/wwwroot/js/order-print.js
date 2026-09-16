window.orderPrint = (() => {
    const windows = new Map();

    return {
        reserve: () => {
            const id = crypto.randomUUID();
            const popup = window.open("about:blank", "_blank");
            if (!popup) return null;
            popup.document.write('<!doctype html><html lang="fa" dir="rtl"><head><meta charset="utf-8"><title>در حال آماده‌سازی</title></head><body style="font-family:Tahoma;padding:2rem">در حال آماده‌سازی نسخه چاپی…</body></html>');
            popup.document.close();
            windows.set(id, popup);
            return id;
        },
        show: (id, html) => {
            const popup = windows.get(id);
            if (!popup || popup.closed) return false;
            popup.document.open();
            popup.document.write(html);
            popup.document.close();
            windows.delete(id);
            return true;
        },
        close: id => {
            const popup = windows.get(id);
            if (popup && !popup.closed) popup.close();
            windows.delete(id);
        }
    };
})();
