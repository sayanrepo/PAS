(async () => {
    if (document.body.dataset.printKind === "bill") {
        $(".autoNumber").autoNumeric("init", {
            aSep: ",", aSign: "", lZero: "deny",
            vMin: "-999999999999999", vMax: "999999999999999"
        });
        // The bill has editable fields to complete before printing.
        return;
    }
    await document.fonts.ready;
    if (document.readyState !== "complete")
        await new Promise(resolve => window.addEventListener("load", resolve, { once: true }));
    window.print();
})();
