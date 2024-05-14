
using System.Diagnostics;
using Godot;

public class TestSimpleBot : TestClass<TestSimpleBot>
{
    [Test]
    public void GetAction_ShouldMoveDownWhenBallIsBelow()
    {
        // given
        SimpleBot bot = MockSimpleBot.Get();
        double paddlePosition = 0.0f;
        Vector2 ballPosition = new Vector2(0.0f, 100.0f);
        Vector2 ballVelocity = Vector2.Zero;

        // when
        Action action = bot.GetAction(paddlePosition: 0.0f, ballPosition: ballPosition, ballVelocity: ballVelocity);

        // then
        Assert.IsTrue(action == Action.Down); // TODO writing own correctly working assert is kinda pointless
    }
    
    [Test]
    public void GetAction_ShouldMoveUpWhenBallIsAbove()
    {
        // given
        SimpleBot bot = MockSimpleBot.Get();
        double paddlePosition = 0.0f;
        Vector2 ballPosition = new Vector2(0.0f, -100.0f);
        Vector2 ballVelocity = Vector2.Zero;

        // when
        Action action = bot.GetAction(paddlePosition: 0.0f, ballPosition: ballPosition, ballVelocity: ballVelocity);

        // then
        Assert.IsTrue(action == Action.Up); // TODO writing own correctly working assert is kinda pointless
    }

    private class MockSimpleBot : SimpleBot
    {
        public static MockSimpleBot Get() => new();
    }
}