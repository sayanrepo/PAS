window.orderPrint = {
    syncSession: async ticket => {
        const url = new URL("print/session", document.baseURI);
        const tokenResponse = await fetch(url, { credentials: "same-origin", cache: "no-store" });
        if (!tokenResponse.ok) throw new Error("آماده‌سازی نشست چاپ ناموفق بود.");
        const { requestToken } = await tokenResponse.json();
        const response = await fetch(url, {
            method: "POST",
            credentials: "same-origin",
            headers: { "X-CSRF-TOKEN": requestToken },
            body: new URLSearchParams({ ticket: ticket ?? "" })
        });
        if (!response.ok) throw new Error("آماده‌سازی نشست چاپ ناموفق بود.");
    }
};
