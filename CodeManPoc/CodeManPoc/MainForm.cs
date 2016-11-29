using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CodeManPoc
{
    public partial class MainForm : Form
    {
        private MapRendering _rendering;
        private GameMap _map;
        private Game _game;
        private Random _rnd;

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            _rendering = new MapRendering(boardPic);
            _rnd = new Random();
            ResetGame();
            _rendering.Render(_map);
        }

        private void ResetGame()
        {
            _map = new GameMap("map.txt");
            _game = new Game(_map);
        }

        private void upBtn_Click(object sender, EventArgs e)
        {
            movePacman(-1, 0);
        }

        private void downBtn_Click(object sender, EventArgs e)
        {
            movePacman(1, 0);
        }

        private void leftBtn_Click(object sender, EventArgs e)
        {
            movePacman(0, -1);
        }

        private void rightBtn_Click(object sender, EventArgs e)
        {
            movePacman(0, 1);
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Left:
                case Keys.J:
                    leftBtn.PerformClick();
                    break;
                case Keys.Right:
                case Keys.L:
                    rightBtn.PerformClick();
                    break;
                case Keys.Up:
                case Keys.I:
                    upBtn.PerformClick();
                    break;
                case Keys.Down:
                case Keys.K:
                    downBtn.PerformClick();
                    break;
                default:
                    return;
            }
            e.Handled = true;
        }

        private void movePacman(int rdiff, int cdiff)
        {
            int row = _map.Pacman.Row + rdiff;
            int col = _map.Pacman.Col + cdiff;
            if (row < 0 || row >= _map.Height)
                return;
            if (col < 0 || col >= _map.Width)
                return;
            if (_map[row, col] == MapTileType.WALL)
                return;
            _game.Pacman = new MapPoint(row, col);
        }

        private int weighType(GameSight sight)
        {
            switch (sight.Obstacle)
            {
                case MapTileType.POINT:
                    return 0;
                case MapTileType.WALL:
                    return 1;
                case MapTileType.GHOST1:
                    return 2;
                default:
                    return 3;
            }
        }

        private void lookRay(List<GameSight> rays, int rdiff, int cdiff)
        {
            int row = _map.Pacman.Row + rdiff;
            int col = _map.Pacman.Col + cdiff;
            int distance = 0;
            while (row >= 0 && row < _map.Height && col >= 0 && col < _map.Width)
            {
                if (_map[row, col] == MapTileType.WALL)
                {
                    if (distance > 0)
                    {
                        rays.Add(new GameSight
                        {
                            Position = new MapPoint(_map.Pacman.Row + rdiff, _map.Pacman.Col + cdiff),
                            Distance = distance,
                            Obstacle = MapTileType.WALL
                        });
                    }
                    return;
                }
                if (_map.Ghost.Any(g => g.Row == row && g.Col == col))
                {
                    rays.Add(new GameSight
                    {
                        Position = new MapPoint(_map.Pacman.Row + rdiff, _map.Pacman.Col + cdiff),
                        Distance = distance,
                        Obstacle = MapTileType.GHOST1
                    });
                    return;
                }
                if (_map[row, col] == MapTileType.POINT)
                {
                    rays.Add(new GameSight
                    {
                        Position = new MapPoint(_map.Pacman.Row + rdiff, _map.Pacman.Col + cdiff),
                        Distance = distance,
                        Obstacle = MapTileType.POINT
                    });
                    return;
                }
                distance++;
                row += rdiff;
                col += cdiff;
            }
            if (distance > 0)
            {
                rays.Add(new GameSight
                {
                    Position = new MapPoint(_map.Pacman.Row + rdiff, _map.Pacman.Col + cdiff),
                    Distance = distance,
                    Obstacle = MapTileType.WALL
                });
            }
        }

        private void randomGhosts()
        {
            for (int i = 0; i < 4; i++)
            {
                List<MapPoint> points = new List<MapPoint>();
                checkGhostPoint(points, i, -1, 0);
                checkGhostPoint(points, i, 1, 0);
                checkGhostPoint(points, i, 0, -1);
                checkGhostPoint(points, i, 0, 1);
                if (points.Count == 1)
                {
                    _game.Ghost[i] = points[0];
                }
                else if (points.Count > 0)
                {
                    _game.Ghost[i] = points[_rnd.Next(points.Count)];
                }
                else
                {
                    _game.Ghost[i] = _game.Trail[i];
                }
            }
        }

        private void proxyGhosts()
        {
            for (int i = 0; i < 4; i++)
            {
                List<MapPoint> points = new List<MapPoint>();
                checkGhostPoint(points, i, -1, 0);
                checkGhostPoint(points, i, 1, 0);
                checkGhostPoint(points, i, 0, -1);
                checkGhostPoint(points, i, 0, 1);
                if (points.Count == 1)
                {
                    _game.Ghost[i] = points[0];
                }
                else if (points.Count > 2)
                {
                    _game.Ghost[i] = points[_rnd.Next(points.Count)];
                }
                else if (points.Count > 0)
                {
                    Tuple<int, int>[] values = new Tuple<int, int>[points.Count];
                    for (int j = 0; j < values.Length; j++)
                    {
                        int v = 0;
                        if ((points[j].Row - _map.Ghost[i].Row) * (_map.Pacman.Row - _map.Ghost[i].Row) > 0)
                            v++;
                        if ((points[j].Col - _map.Ghost[i].Col) * (_map.Pacman.Col - _map.Ghost[i].Col) > 0)
                            v++;
                        values[j] = Tuple.Create(v, j);
                    }
                    Array.Sort(values, (a, b) => b.Item1 - a.Item1);
                    int good = values[0].Item1;
                    values = values.Where(t => t.Item1 == good).ToArray();
                    _game.Ghost[i] = points[values[_rnd.Next(values.Length)].Item2];
                }
                else
                {
                    _game.Ghost[i] = _game.Trail[i];
                }
            }
        }

        private int shortestPathToPacman(MapPoint p)
        {
            int[,] crumbs = new int[_map.Height, _map.Width];
            for (int row = 0; row < _map.Height; row++)
            {
                for (int col = 0; col < _map.Width; col++)
                {
                    crumbs[row, col] = 1000000;
                }
            }
            Queue<MapPoint> front = new Queue<MapPoint>();
            crumbs[p.Row, p.Col] = 0;
            front.Enqueue(p);
            while (front.Count > 0)
            {
                MapPoint x = front.Dequeue();
                for (int dr = -1; dr <= 1; dr++)
                {
                    for (int dc = -1; dc <= 1; dc++)
                    {
                        if (dr * dc != 0 || dr + dc == 0)
                            continue;
                        MapPoint y = new MapPoint(x.Row + dr, x.Col + dc);
                        if (y.Row < 0 || y.Row >= _map.Height)
                            continue;
                        if (y.Col < 0 || y.Col >= _map.Width)
                            continue;
                        if (_map[y] == MapTileType.WALL)
                            continue;
                        int v = crumbs[x.Row, x.Col] + 1;
                        if (crumbs[y.Row, y.Col] <= v)
                            continue;
                        crumbs[y.Row, y.Col] = v;
                        if (y.Equals(_map.Pacman))
                            return v;
                        front.Enqueue(y);
                    }
                }
            }
            return 2000000;
        }

        private void shortGhosts()
        {
            for (int i = 0; i < 4; i++)
            {
                List<MapPoint> points = new List<MapPoint>();
                checkGhostPoint(points, i, -1, 0);
                checkGhostPoint(points, i, 1, 0);
                checkGhostPoint(points, i, 0, -1);
                checkGhostPoint(points, i, 0, 1);
                if (points.Count == 1)
                {
                    _game.Ghost[i] = points[0];
                }
                else if (points.Count > 0)
                {
                    Tuple<int,int>[] values = new Tuple<int,int>[points.Count];
                    for (int j = 0; j < values.Length; j++) {
                        values[j] = Tuple.Create(shortestPathToPacman(points[j]), j);
                    }
                    Array.Sort(values, (a, b) => a.Item1 - b.Item1);
                    int good = values[0].Item1;
                    values = values.Where(t => t.Item1 == good).ToArray();
                    _game.Ghost[i] = points[values[_rnd.Next(values.Length)].Item2];
                }
                else
                {
                    _game.Ghost[i] = _game.Trail[i];
                }
            }
        }

        private void gameTimer_Tick(object sender, EventArgs e)
        {
            // Move ghosts
            if (ghostRandomRadio.Checked)
                randomGhosts();
            else if (ghostProxyRadio.Checked)
                proxyGhosts();
            else if (ghostShortRadio.Checked)
                shortGhosts();
            else
                randomGhosts();

            _game.Trail = _map.Ghost;
            _map.Ghost = _game.Ghost.ToArray();

            // Move pacman
            if (pacmanAutoChk.Checked)
            {
                // Find nearest osbtacle in a line of sight in all directions
                List<GameSight> rays = new List<GameSight>();
                lookRay(rays, -1, 0);
                lookRay(rays, 1, 0);
                lookRay(rays, 0, -1);
                lookRay(rays, 0, 1);
                rays.Sort((a, b) =>
                {
                    int diff = weighType(a) - weighType(b);
                    if (diff != 0)
                        return diff;
                    if (a.Obstacle == MapTileType.GHOST1)
                        return b.Distance - a.Distance;
                    else
                        return a.Distance - b.Distance;
                });
                if (rays.Count > 0)
                {
                    if (rays[0].Obstacle == MapTileType.WALL && rays.Count(t => t.Obstacle == MapTileType.WALL) >= 2)
                    {
                        rays.RemoveAll(t => t.Obstacle != MapTileType.WALL || t.Position.Equals(_game.CameFrom));
                        _game.CameFrom = _map.Pacman;
                        if (rays.Count == 1)
                            _map.Pacman = rays[0].Position;
                        else
                            _map.Pacman = rays[_rnd.Next(rays.Count)].Position;
                    }
                    else
                    {
                        _game.CameFrom = _map.Pacman;
                        _map.Pacman = rays[0].Position;
                    }
                }
            }
            else
            {
                _game.CameFrom = _map.Pacman;
                _map.Pacman = _game.Pacman;
            }



            bool gameLost = false;

            // if ghost hits pacman, lose the game
            if (_map.Ghost.Any(g => g.Equals(_map.Pacman)))
            {
                // LOSE
                Text = "Codeman LOST";
                gameTimer.Enabled = false;
                gameLost = true;
            }

            if (!gameLost)
            {
                // if pacman is on point, eat it
                if (_map[_map.Pacman] == MapTileType.POINT)
                {
                    _map[_map.Pacman] = MapTileType.EMPTY;
                    _game.PointCount--;
                }

                if (_game.PointCount == 0)
                {
                    // WIN
                    Text = "Codeman WON";
                    gameTimer.Enabled = false;
                }
            }

            _rendering.Render(_map);
        }

        private void checkGhostPoint(List<MapPoint> points, int i, int rdiff, int cdiff)
        {
            MapPoint p = new MapPoint(_map.Ghost[i].Row + rdiff, _map.Ghost[i].Col + cdiff);
            if (p.Row < 0 || p.Row >= _map.Height)
                return;
            if (p.Col < 0 || p.Col >= _map.Width)
                return;
            if (p.Equals(_game.Trail[i]))
                return;
            if (_map[p] == MapTileType.WALL)
                return;
            points.Add(p);
        }

        private void playBtn_Click(object sender, EventArgs e)
        {
            ResetGame();
            _rendering.Render(_map);
            gameTimer.Enabled = true;
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            _rendering.Render(_map);
        }
    }
}
