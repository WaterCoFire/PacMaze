namespace Entity.Prop {
    /**
     * Prop type enum
     * Pacboy/Ghostron are all seen as special props
     * (as they are "placed" like any other props during game initialisation)
     */
    public enum PropType {
        None, // Useful for default or empty cases
        Pacboy,
        Ghostron, // Generic Ghostron type for placement logic
        PowerPellet,
        FastWheel,
        NiceBomb,
        SlowWheel,
        BadCherry,
        LuckyDice
    }
}