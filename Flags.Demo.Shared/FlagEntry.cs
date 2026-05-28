using Flags.Icons;

namespace Flags.Demo.Shared {
    /// <summary>
    /// Demo-grid row. Exactly one of <see cref="Twemoji"/>, <see cref="Circle"/>, <see cref="Square"/>,
    /// <see cref="Lipis"/>, <see cref="FlagHub"/> is non-<c>None</c> — XAML can blindly bind all five to a
    /// single <c>FlagIcon</c> and the control will pick the active one.
    /// </summary>
    public class FlagEntry {
        public FlagEntry(TwemojiFlag flag) {
            Source = FlagSource.Twemoji;
            Twemoji = flag;
            Code = flag.ToString();
            Snippet = "TwemojiFlag." + flag;
        }
        public FlagEntry(CircleFlag flag) {
            Source = FlagSource.Circle;
            Circle = flag;
            Code = flag.ToString();
            Snippet = "CircleFlag." + flag;
        }
        public FlagEntry(SquareFlag flag) {
            Source = FlagSource.Square;
            Square = flag;
            Code = flag.ToString();
            Snippet = "SquareFlag." + flag;
        }
        public FlagEntry(LipisFlag flag) {
            Source = FlagSource.Lipis;
            Lipis = flag;
            Code = flag.ToString();
            Snippet = "LipisFlag." + flag;
        }
        public FlagEntry(FlagHubFlag flag) {
            Source = FlagSource.FlagHub;
            FlagHub = flag;
            Code = flag.ToString();
            Snippet = "FlagHubFlag." + flag;
        }

        public FlagSource Source { get; }
        public string Code { get; }
        public string Snippet { get; }

        public TwemojiFlag Twemoji { get; }
        public CircleFlag Circle { get; }
        public SquareFlag Square { get; }
        public LipisFlag Lipis { get; }
        public FlagHubFlag FlagHub { get; }
    }
}
