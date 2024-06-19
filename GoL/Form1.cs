namespace GoL;

#pragma warning disable IDE2001

public partial class Form1 : Form
{
    const int ARR_SIZE = 10;
    const float ARR_SIZEF = ARR_SIZE;

    readonly Cell[,] _arr;
    readonly Pen _pen;
    readonly Brush _brush;

    private SizeF _spaceSize;

    public Form1()
    {
        InitializeComponent();

        DoubleBuffered = true;
        Paint += Form1_Paint;
        ResizeEnd += Form1_ResizeEnd;
        MouseClick += Form1_MouseClick;
        KeyDown += Form1_KeyDown;

        _arr = new Cell[ARR_SIZE, ARR_SIZE];
        _pen = Pens.Black;
        _brush = Brushes.Black;
        _spaceSize = new SizeF(ClientSize.Width / ARR_SIZEF, ClientSize.Height / ARR_SIZEF);

        for (int x = 0, y = 0; x < ARR_SIZE; y++)
        {
            if (y >= ARR_SIZE)
            {
                y = -1;
                x++;
                continue;
            }

            _arr[x, y] = new Cell()
            {
                Rect = new RectangleF(x * _spaceSize.Width, y * _spaceSize.Height, _spaceSize.Width, _spaceSize.Height),
            };
        }
    }

    private void Form1_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Space)
            UpdateGrid();
    }

    private void Form1_MouseClick(object? sender, MouseEventArgs e)
    {
        float fx = e.X - _spaceSize.Width;
        float fy = e.Y - _spaceSize.Height;

        for (int x = 0, y = 0; x < ARR_SIZE; y++)
        {
            if (y >= ARR_SIZE)
            {
                y = -1;
                x++;
                continue;
            }

            if (_arr[x, y].Rect.X >= fx && _arr[x, y].Rect.Y >= fy)
            {
                _arr[x, y].Enabled = !_arr[x, y].Enabled;
                break;
            }
        }
        Invalidate();
    }

    private void Form1_ResizeEnd(object? sender, EventArgs e)
    {
        _spaceSize = new SizeF(ClientSize.Width / ARR_SIZEF, ClientSize.Height / ARR_SIZEF);

        for (int x = 0, y = 0; x < ARR_SIZE; y++)
        {
            if (y >= ARR_SIZE)
            {
                y = -1;
                x++;
                continue;
            }

            _arr[x, y].Rect = new RectangleF(x * _spaceSize.Width, y * _spaceSize.Height, _spaceSize.Width, _spaceSize.Height);
        }

        Invalidate();
    }

    private void Form1_Paint(object? sender, PaintEventArgs e)
    {
        for (int x = 0, y = 0; x < ARR_SIZE; y++)
        {
            if (y >= ARR_SIZE)
            {
                y = -1;
                x++;
                continue;
            }

            DrawSquare(e.Graphics, x, y);
        }
    }

    /*
        For a space that is 'populated':
            Each cell with one or no neighbors dies, as if by solitude.
            Each cell with four or more neighbors dies, as if by overpopulation.
            Each cell with two or three neighbors survives.
        For a space that is 'empty' or 'unpopulated'
            Each cell with three neighbors becomes populated.
    */

    private void UpdateGrid()
    {
        Cell[,] narr = (Cell[,])_arr.Clone();

        for (int x = 0, y = 0; x < ARR_SIZE; y++)
        {
            if (y >= ARR_SIZE)
            {
                y = -1;
                x++;
                continue;
            }

            int i = 0;

            if (x - 1 >= 0 && y - 1 >= 0 && _arr[x - 1, y - 1].Enabled) i++;
            if (x - 1 >= 0 && _arr[x - 1, y].Enabled) i++;
            if (x - 1 >= 0 && y + 1 < ARR_SIZE && _arr[x - 1, y + 1].Enabled) i++;

            if (y - 1 >= 0 && _arr[x, y - 1].Enabled) i++;
            if (y + 1 < ARR_SIZE && _arr[x, y + 1].Enabled) i++;

            if (x + 1 < ARR_SIZE && y - 1 >= 0 && _arr[x + 1, y - 1].Enabled) i++;
            if (x + 1 < ARR_SIZE && _arr[x + 1, y].Enabled) i++;
            if (x + 1 < ARR_SIZE && y + 1 < ARR_SIZE && _arr[x + 1, y + 1].Enabled) i++;

            if (_arr[x, y].Enabled)
            {
                if (i <= 1 || i >= 4)
                    narr[x, y].Enabled = false;
                else
                    narr[x, y].Enabled = true;
            }
            else if (i == 3)
            {
                narr[x, y].Enabled = true;
            }
        }

        for (int x = 0, y = 0; x < ARR_SIZE; y++)
        {
            if (y >= ARR_SIZE)
            {
                y = -1;
                x++;
                continue;
            }

            _arr[x, y] = narr[x, y];
        }

        Invalidate();
    }

    private void DrawSquare(Graphics g, int x, int y)
    {
        if (_arr[x, y].Enabled)
            g.FillRectangle(_brush, _arr[x, y].Rect);

        g.DrawRectangle(_pen, _arr[x, y].Rect.X, _arr[x, y].Rect.Y, _arr[x, y].Rect.Width, _arr[x, y].Rect.Height);
    }

    private struct Cell
    {
        public RectangleF Rect { get; set; }
        public bool Enabled { get; set; }
    }
}
