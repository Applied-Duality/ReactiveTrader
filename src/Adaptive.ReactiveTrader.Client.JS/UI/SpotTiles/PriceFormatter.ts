class PriceFormatter {
    static getFormattedPrice(rate: number, precision: number, pipsPosition: number) {
        var rateAsString = rate.toFixed(precision);

        var dotIndex = rateAsString.indexOf(".");

        var bigFigures = rateAsString.substring(0, dotIndex + pipsPosition - 1);
        var pips = rateAsString.substring(dotIndex + pipsPosition - 1, dotIndex + pipsPosition + 1);

        var tenthOfPips = "";

        if (precision > pipsPosition)
        {
            tenthOfPips = rateAsString.substring(dotIndex + pipsPosition + 1, rateAsString.length);
        }

        return new FormattedPrice(bigFigures, pips, tenthOfPips);
    }

    static getFormattedSpread(spread: number, precision: number, pipsPosition: number) {
        var delta = precision - pipsPosition;
        if (delta > 0)
        {
            return spread.toFixed(delta);
        }
        return spread.toString();
    }
}
