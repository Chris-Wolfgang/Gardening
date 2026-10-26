namespace Wolfgang.Gardening.Calculator.Tests.Unit;

public class StockPackerTests
{
    private static readonly LumberKind Wall = new(BoardProfile.TwoBySix, Treatment.PressureTreated);

    private static readonly LumberKind Post = new(BoardProfile.TwoByTwo, Treatment.PressureTreated);



    private static CutPiece Pieces(decimal length, int quantity) => new(Wall, length, quantity, PiecePurpose.LengthWall);



    [Fact]
    public void Pack_when_two_4_ft_pieces_and_8_ft_stock_cuts_one_board_with_no_leftover()
    {
        var eight = new StockOption(Wall, 96m, 10m);

        var result = new StockPacker().Pack([Pieces(48m, 2)], [eight]);

        var board = Assert.Single(result.Boards);
        Assert.Equal(eight, board.Stock);
        Assert.Equal([48m, 48m], board.Cuts);
        Assert.Equal(0m, board.LeftoverInches);
    }



    [Fact]
    public void Pack_when_bed_is_8_by_4_ft_two_courses_buys_six_8_ft_boards()
    {
        var bed = new StackedBoardBed(BoardProfile.TwoBySix, Treatment.PressureTreated, 96m, 48m, 2, cornerPost: null);
        var eight = new StockOption(Wall, 96m, 10m);
        StockOption[] stock = [eight, new StockOption(Wall, 120m, 13m), new StockOption(Wall, 144m, 16m)];

        var result = new StockPacker().Pack(bed.CutList(), stock);

        Assert.Equal([new StockPurchase(eight, 6)], result.Purchases);
        Assert.Equal(60m, result.TotalCost);
    }



    [Fact]
    public void Pack_when_a_longer_board_holds_more_pieces_for_less_picks_the_cheaper_plan()
    {
        var ten = new StockOption(Wall, 120m, 11m);

        var result = new StockPacker().Pack([Pieces(60m, 2)], [new StockOption(Wall, 96m, 10m), ten]);

        Assert.Equal([new StockPurchase(ten, 1)], result.Purchases);
    }



    [Fact]
    public void Pack_when_a_shorter_stock_length_holds_the_cuts_buys_the_shorter_board()
    {
        var four = new StockOption(Wall, 48m, 5m);

        var result = new StockPacker().Pack([Pieces(30m, 1)], [new StockOption(Wall, 96m, 10m), four]);

        Assert.Equal([new StockPurchase(four, 1)], result.Purchases);
        Assert.Equal(18m, result.Boards[0].LeftoverInches);
    }



    [Fact]
    public void Pack_when_two_plans_cost_the_same_picks_the_one_with_fewer_boards()
    {
        var eight = new StockOption(Wall, 96m, 10m);

        var result = new StockPacker().Pack([Pieces(48m, 2)], [new StockOption(Wall, 48m, 5m), eight]);

        Assert.Equal([new StockPurchase(eight, 1)], result.Purchases);
    }



    [Fact]
    public void Pack_when_kerf_leaves_no_room_for_a_second_piece_uses_two_boards()
    {
        var eight = new StockOption(Wall, 96m, 10m);

        var result = new StockPacker(kerfInches: 0.125m).Pack([Pieces(48m, 2)], [eight]);

        Assert.Equal([new StockPurchase(eight, 2)], result.Purchases);
    }



    [Fact]
    public void Pack_when_pieces_are_different_kinds_cuts_each_from_its_own_stock()
    {
        var wallStock = new StockOption(Wall, 96m, 10m);
        var postStock = new StockOption(Post, 96m, 4m);
        CutPiece[] pieces = [Pieces(48m, 2), new CutPiece(Post, 11m, 4, PiecePurpose.CornerPost)];

        var result = new StockPacker().Pack(pieces, [wallStock, postStock]);

        Assert.Equal([new StockPurchase(wallStock, 1), new StockPurchase(postStock, 1)], result.Purchases);
        Assert.Equal(14m, result.Purchases.Sum(purchase => purchase.Cost));
    }



    [Fact]
    public void Pack_when_no_pieces_returns_nothing_to_buy()
    {
        var result = new StockPacker().Pack([Pieces(48m, 0)], [new StockOption(Wall, 96m, 10m)]);

        Assert.Empty(result.Boards);
        Assert.Equal(0m, result.TotalCost);
    }



    [Fact]
    public void Pack_when_no_stock_is_long_enough_throws_InvalidOperationException()
    {
        Assert.Throws<InvalidOperationException>
        (
            () => new StockPacker().Pack([Pieces(100m, 1)], [new StockOption(Wall, 96m, 10m)])
        );
    }



    [Fact]
    public void Pack_when_pieces_is_null_throws_ArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new StockPacker().Pack(null!, []));
    }



    [Fact]
    public void Pack_when_stock_is_null_throws_ArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new StockPacker().Pack([], null!));
    }



    [Fact]
    public void Constructor_when_kerf_is_negative_throws_ArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new StockPacker(kerfInches: -0.1m));
    }



    [Fact]
    public void KerfInches_when_constructed_returns_the_input()
    {
        Assert.Equal(0.125m, new StockPacker(kerfInches: 0.125m).KerfInches);
    }



    [Fact]
    public void PackingResult_when_boards_is_null_throws_ArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new PackingResult(null!));
    }



    [Fact]
    public void StockOption_when_kind_is_null_throws_ArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new StockOption(null!, 96m, 10m));
    }



    [Theory]
    [InlineData(0, 10)]
    [InlineData(96, -1)]
    public void StockOption_when_value_is_out_of_range_throws_ArgumentOutOfRangeException(decimal length, decimal price)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new StockOption(Wall, length, price));
    }
}
