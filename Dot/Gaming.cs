namespace DotsAndBoxes.Dot
{
    public static class Gaming
    {
        public enum Direction
        {
            UP,
            DOWN,
            LEFT,
            RIGHT
        }
        public static int PictureHeight, PictureWidth; //width为x轴 Height为y轴（以左上角为原点）
        public static int[,] SquareStatus; //每个方格的状态

        public static void Init()
        {
            SquareStatus = new int[PictureWidth - 1, PictureHeight - 1];
            for (var i = 0; i < PictureWidth - 1; i++) {
                for (var j = 0; j < PictureHeight - 1; j++) {
                    SquareStatus[i, j] = 1111; //顺序上下左右  ↑↓←→
                }
            }
        }

        public static bool ChangeSquareStatus(int x, int y, Direction direction)
        {
            if (!IsClickable(x, y, direction)) return false; //改变失败
            switch (direction) {
                case Direction.UP:
                    SquareStatus[x - 1, y - 1] = SquareStatus[x - 1, y - 1] - 1000;
                    if (y != 1) {
                        SquareStatus[x - 1, y - 2] = SquareStatus[x - 1, y - 2] - 100;
                    }
                    break;
                case Direction.DOWN:
                    SquareStatus[x - 1, y - 1] = SquareStatus[x - 1, y - 1] - 100;
                    if (y != PictureWidth - 1) {
                        SquareStatus[x - 1, y] = SquareStatus[x - 1, y] - 1000;
                    }
                    break;
                case Direction.LEFT:
                    SquareStatus[x - 1, y - 1] = SquareStatus[x - 1, y - 1] - 10;
                    if (x != 1) {
                        SquareStatus[x - 2, y - 1] = SquareStatus[x - 2, y - 1] - 1;
                    }
                    break;
                case Direction.RIGHT:
                    SquareStatus[x - 1, y - 1] = SquareStatus[x - 1, y - 1] - 1;
                    if (x != PictureHeight - 1) {
                        SquareStatus[x, y - 1] = SquareStatus[x, y - 1] - 10;
                    }
                    break;
            }
            return true; //改变成功
        }

        public static bool IsClickable(int x, int y, Direction direction)
        {
            var temp = SquareStatus[x - 1, y - 1];
            var right = temp%10;
            temp = temp - temp%10;
            var left = temp%100/10;
            temp = temp - left*10;
            var down = temp%1000/100;
            temp = temp - down*100;
            var up = temp/1000;
            switch (direction) {
                case Direction.UP:
                    if (up == 1) {
                        return true;
                    }
                    break;
                case Direction.DOWN:
                    if (down == 1) {
                        return true;
                    }
                    break;
                case Direction.LEFT:
                    if (left == 1) {
                        return true;
                    }
                    break;
                case Direction.RIGHT:
                    if (right == 1) {
                        return true;
                    }
                    break;
            }
            return false;
        }

        public static void JudgeChangeColor(int x, int y, Direction direction, int playerNumber)
        {
            switch (direction) {
                case Direction.UP:
                    if (SquareStatus[x - 1, y - 1] == 0) {
                        ChangeColor(playerNumber, x, y);
                    }
                    if (y != 1) {
                        if (SquareStatus[x - 1, y - 2] == 0) {
                            ChangeColor(playerNumber, x, y - 1);
                        }
                    }
                    break;
                case Direction.DOWN:
                    if (SquareStatus[x - 1, y - 1] == 0) {
                        ChangeColor(playerNumber, x, y);
                    }
                    if (y != PictureWidth - 1) {
                        if (SquareStatus[x - 1, y] == 0) {
                            ChangeColor(playerNumber, x, y + 1);
                        }
                    }
                    break;
                case Direction.LEFT:
                    if (SquareStatus[x - 1, y - 1] == 0) {
                        ChangeColor(playerNumber, x, y);
                    }
                    if (x != 1) {
                        if (SquareStatus[x - 2, y - 1] == 0) {
                            ChangeColor(playerNumber, x - 1, y);
                        }
                    }
                    break;
                case Direction.RIGHT:
                    if (SquareStatus[x - 1, y - 1] == 0) {
                        ChangeColor(playerNumber, x, y);
                    }
                    if (x != PictureHeight - 1) {
                        if (SquareStatus[x, y - 1] == 0) {
                            ChangeColor(playerNumber, x + 1, y);
                        }
                    }
                    break;
            }
        }

        public static void ChangeColor(int playerNumber, int x, int y)
        {
            SquareStatus[x - 1, y - 1] = playerNumber + 2;
            //变色 这个格子变成玩家X的
        }

        public static int? JudgeResult()
        {
            int playerOneScore = 0, playerTwoScore = 0;
            for (var i = 0; i < PictureWidth - 1; i++) {
                for (var j = 0; j < PictureHeight - 1; j++) {
                    if (SquareStatus[i, j] == 3) {
                        playerOneScore++;
                    } else {
                        if (SquareStatus[i, j] == 4) {
                            playerTwoScore++;
                        } else {
                            return null; //游戏未结束
                        }
                    }
                }
            }
            if (playerOneScore > playerTwoScore) {
                return 1; //此玩家获胜
            }
            if (playerOneScore != playerTwoScore) {
                return -1; //对方获胜
            }
            return 0; //平局
        }
    }
}