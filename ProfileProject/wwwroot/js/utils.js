function formatValue(value, tableKey) {
    const isNegative = value < 0;
    let absValue = Math.abs(value);

    if (['vade', 'bayi'].includes(tableKey)) {
        return (isNegative ? '-' : '') + absValue.toLocaleString('de-DE');
    }

    let displayValue = "";

    if (absValue == null) {
        displayValue = "0 $";
    } else if (absValue >= 1000000000) {
        displayValue = (absValue / 1000000000).toFixed(1) + "B";
    } else if (absValue >= 1000000) {
        displayValue = (absValue / 1000000).toFixed(1) + "M";
    } else if (absValue >= 1000) {
        displayValue = (absValue / 1000).toFixed(1) + "K";
    } else if (absValue >= 100) {
        displayValue = Math.floor(absValue).toFixed(1) + "K";
    } else {
        displayValue = Math.floor(absValue).toString();
    }

    return (isNegative ? '-' : '') + displayValue.toLocaleString('de-DE') + ' $';
}