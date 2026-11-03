namespace Wolfgang.Gardening.Calculator;

/// <summary>
/// A rectangular raised bed whose walls are boards stacked in courses.
/// </summary>
/// <remarks>
/// <para>
/// Each course is four boards laid as a pinwheel: every board butts into the face of the next, and the
/// rotation alternates from course to course so the corner seams are staggered. A course is therefore two
/// boards of <see cref="LengthInches"/> and two of <see cref="WidthInches"/>, the lengths the user enters.
/// </para>
/// <para>
/// Because each board covers one board thickness of the corner, the outside footprint is one thickness
/// larger than the entered lengths and the inside is one thickness smaller.
/// </para>
/// </remarks>
public sealed class StackedBoardBed
{
    /// <summary>
    /// Initializes a new <see cref="StackedBoardBed"/>.
    /// </summary>
    /// <param name="board">The wall board profile.</param>
    /// <param name="treatment">Whether the wall boards (and corner posts) are pressure treated.</param>
    /// <param name="lengthInches">The length of the boards along the bed's length, in inches.</param>
    /// <param name="widthInches">The length of the boards along the bed's width, in inches.</param>
    /// <param name="courses">How many boards high the walls are.</param>
    /// <param name="cornerPost">The corner post profile, or <see langword="null"/> for no corner posts.</param>
    /// <exception cref="ArgumentNullException"><paramref name="board"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="lengthInches"/> or <paramref name="widthInches"/> is not longer than the board
    /// thickness, or <paramref name="courses"/> is less than one.
    /// </exception>
    public StackedBoardBed
    (
        BoardProfile board,
        Treatment treatment,
        decimal lengthInches,
        decimal widthInches,
        int courses,
        BoardProfile? cornerPost
    )
    {
        ArgumentNullException.ThrowIfNull(board);
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(lengthInches, board.ThicknessInches);
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(widthInches, board.ThicknessInches);
        ArgumentOutOfRangeException.ThrowIfLessThan(courses, 1);

        Board = board;
        Treatment = treatment;
        LengthInches = lengthInches;
        WidthInches = widthInches;
        Courses = courses;
        CornerPost = cornerPost;
    }



    /// <summary>The wall board profile.</summary>
    public BoardProfile Board { get; }



    /// <summary>Whether the wall boards and corner posts are pressure treated.</summary>
    public Treatment Treatment { get; }



    /// <summary>The length of the boards along the bed's length, in inches.</summary>
    public decimal LengthInches { get; }



    /// <summary>The length of the boards along the bed's width, in inches.</summary>
    public decimal WidthInches { get; }



    /// <summary>How many boards high the walls are.</summary>
    public int Courses { get; }



    /// <summary>The corner post profile, or <see langword="null"/> when the bed has no corner posts.</summary>
    public BoardProfile? CornerPost { get; }



    /// <summary>The wall height in inches: the number of courses times the board width.</summary>
    public decimal HeightInches => Courses * Board.WidthInches;



    /// <summary>The outside length of the bed in inches.</summary>
    public decimal OutsideLengthInches => LengthInches + Board.ThicknessInches;



    /// <summary>The outside width of the bed in inches.</summary>
    public decimal OutsideWidthInches => WidthInches + Board.ThicknessInches;



    /// <summary>The inside (planting) length of the bed in inches.</summary>
    public decimal InsideLengthInches => LengthInches - Board.ThicknessInches;



    /// <summary>The inside (planting) width of the bed in inches.</summary>
    public decimal InsideWidthInches => WidthInches - Board.ThicknessInches;



    /// <summary>The number of board ends: two per board, four boards per course.</summary>
    public int BoardEnds => Courses * 4 * 2;



    /// <summary>
    /// Builds the cut list: the wall boards for every course and, when the bed has them, four corner posts
    /// as tall as the wall.
    /// </summary>
    /// <returns>The pieces the bed needs.</returns>
    public IReadOnlyList<CutPiece> CutList()
    {
        var wall = new LumberKind(Board, Treatment);

        var pieces = new List<CutPiece>
        {
            new(wall, LengthInches, 2 * Courses, PiecePurpose.LengthWall),
            new(wall, WidthInches, 2 * Courses, PiecePurpose.WidthWall),
        };

        if (CornerPost is not null)
        {
            pieces.Add(new CutPiece(new LumberKind(CornerPost, Treatment), HeightInches, 4, PiecePurpose.CornerPost));
        }

        return pieces;
    }
}
