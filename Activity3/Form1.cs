using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.Drawing.Drawing2D;

namespace Activity3
{
    public partial class Form1 : Form
    {
        int side;
        int numX = 20;
        int numY = 16;
        Square[,] grid;

        Square highlighted = new Square();
        Square selected = new Square(0,0);

        //int mode = 0;
        LinkedList<PathNode> tree;
        Stack<PathNode> search;
        int exploreLimit;
        int exploreCount;

        PathNode goalNode;

        public Form1()
        {
            InitializeComponent();

            grid = new Square[numX, numY];
            exploreLimit = numX * numY;
            resetGrid();
            side = Convert.ToInt16(pictureBox1.Width / numX);

            tree = new LinkedList<PathNode>();
            tree.AddFirst(new PathNode(selected));
            search = new Stack<PathNode>();
            search.Push(tree.First.Value);

        }

        public void resetGrid()
        {
            for (int x = 0; x < numX; x++)
            {
                for (int y = 0; y < numY; y++)
                {
                    grid[x, y] = new Square(x,y);
                }
            }
        }

        public bool isValid(int x, int y)
        {
            if (x >= 0 && x < numX && y >= 0 && y < numY)
                return true;
            else
                return false;
        }

        private bool hasArrived(PathNode target)
        {
            //bool arrived = target.location.Equals(highlighted);
            bool arrived = (target.location.X == highlighted.X && target.location.Y == highlighted.Y);
            return arrived;
        }

        private void exploreNode(PathNode target)
        {

            // UP DOWN LEFT RIGHT
            Point UP = new Point(target.location.X, target.location.Y - 1);
            Point DOWN = new Point(target.location.X, target.location.Y + 1);
            Point LEFT = new Point(target.location.X - 1, target.location.Y);
            Point RIGHT = new Point(target.location.X + 1, target.location.Y);

            Point[] direction = { UP, DOWN, LEFT, RIGHT };

            for(int i = 0; i<4 ; i++)
            {
                if (isValid(direction[i].X, direction[i].Y))
                {
                    Square check = grid[direction[i].X, direction[i].Y];
                    if (check.passable && !check.explored)
                    {
                        tree.AddLast(new PathNode(grid[direction[i].X, direction[i].Y], target));
                        search.Push(tree.Last.Value);
                        exploreCount++;
                        grid[target.location.X, target.location.Y].explored = true;
                    }
                }
            }     
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (!checkBox1.Checked)
                DrawPath(e.X, e.Y);

            pictureBox1.Refresh();

            lblMouse.Text = "X: " + e.X + " Y: " + e.Y;
            lblSquare.Text = "( " + highlighted.X + " , " + highlighted.Y + " ) ";
            //if (checkBox1.Checked == false)
            //{
            //    ResetExploredSquare();

            //    highlighted.X = e.X / side;
            //    highlighted.Y = e.Y / side;

            //    exploreCount = 0;

            //    tree = new LinkedList<PathNode>();
            //    tree.AddFirst(new PathNode(selected));
            //    search = new Stack<PathNode>();
            //    search.Push(tree.First.Value);

            //    bool foundGoal = false;

            //    while (search.Count > 0 && !foundGoal)
            //    {
            //        PathNode target = (PathNode)search.Pop();

            //        if (target != null)
            //        {
            //            foundGoal = hasArrived(target);
            //            if (foundGoal)
            //            {
            //                goalNode = target;
            //                lblPath.Text = "PathFound";
            //            }
            //            //else if (exploreCount > exploreLimit)
            //            //{
            //            //    Console.WriteLine("LIMIT");
            //            //    break;
            //            //}
            //            else
            //            {
            //                exploreNode(target);
            //                //listBox1.Items = search;
            //                listBox1.Items.Add($"({target.location.X}, {target.location.Y})");
            //            }
            //        } 
            //    }
            //}
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            for (int x = 0; x < numX; x++)
            {
                for (int y = 0; y < numY; y++)
                {
                    Rectangle s = new Rectangle(x * side, y * side, side, side);

                    if (x == selected.X && y == selected.Y)
                        e.Graphics.FillRectangle(Brushes.SlateBlue, s);
                    else if (x == highlighted.X && y == highlighted.Y)
                    {
                        if(!grid[x, y].passable)
                            e.Graphics.FillRectangle(Brushes.IndianRed, s);
                        else
                            e.Graphics.FillRectangle(Brushes.SkyBlue, s);
                    }
                        
                    else if (!grid[x, y].passable)
                        e.Graphics.FillRectangle(Brushes.Black, s);
                    else
                        e.Graphics.FillRectangle(Brushes.White, s);
                }
            }

            while (goalNode != null)
            {
                Rectangle s = new Rectangle(goalNode.location.X * side,
                                            goalNode.location.Y * side,
                                            side, side);
                e.Graphics.FillRectangle(Brushes.Gray, s);
                goalNode = goalNode.origin;
            }

            for (int x = 0; x < numX; x++)
            {
                for (int y = 0; y < numY; y++)
                {
                    Rectangle s = new Rectangle(x * side, y * side, side, side);

                    if (x == selected.X && y == selected.Y)
                        e.Graphics.FillRectangle(Brushes.SlateBlue, s);
                    else if (x == highlighted.X && y == highlighted.Y)
                    {
                        if (!grid[x, y].passable)
                            e.Graphics.FillRectangle(Brushes.IndianRed, s);
                        else
                            e.Graphics.FillRectangle(Brushes.SkyBlue, s);
                    }
                }
            }

            for (int x = 0; x < numX; x++)
            {
                for (int y = 0; y < numY; y++)
                {
                    Rectangle s = new Rectangle(x * side, y * side, side, side);
                    e.Graphics.DrawRectangle(Pens.Black, s);
                } 
            }
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            int x = e.X / side;
            int y = e.Y / side;

            if (!grid[x, y].passable && checkBox1.Checked)
            {
                grid[x, y].passable = !grid[x, y].passable;
            }

            else if (!grid[x, y].passable)
            {
                return;
            }

            else if (checkBox1.Checked)
            {
                grid[x, y].passable = !grid[x, y].passable;
            }
            else
            {
                selected.X = e.X / side;
                selected.Y = e.Y / side;

                FindPath();

                pictureBox1.Refresh();

                lblSelected.Text = "( " + selected.X + " , " + selected.Y + " ) ";
            }
        }

        private void ResetExploredSquare()
        {
            for(int x = 0; x <numX; x++)
            {
                for (int y = 0; y < numY; y++)
                    grid[x, y].explored = false;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {

        }

        private void DrawPath(int x, int y)
        {
            ResetExploredSquare();

            highlighted.X = x / side;
            highlighted.Y = y / side;

            exploreCount = 0;

            tree = new LinkedList<PathNode>();
            tree.AddFirst(new PathNode(selected));
            search = new Stack<PathNode>();
            search.Push(tree.First.Value);

            bool foundGoal = false;

            listBox1.Items.Clear();

            while (search.Count > 0 && !foundGoal)
            {
                PathNode target = (PathNode)search.Pop();

                if (target != null)
                {
                    foundGoal = hasArrived(target);
                    if (foundGoal)
                    {
                        goalNode = target;
                    }
                    else
                    {
                        exploreNode(target);
                        //listBox1.Items = search;
                        if(!listBox1.Items.Contains($"({target.location.X}, {target.location.Y})"))
                            listBox1.Items.Add($"({target.location.X}, {target.location.Y})");
                    }
                }
            }
        }

        private void FindPath()
        {
            tree = new LinkedList<PathNode>();
            tree.AddFirst(new PathNode(selected));
            search = new Stack<PathNode>();
            search.Push(tree.First.Value);

            bool foundGoal = false;

            while (search.Count > 0 && !foundGoal)
            {
                PathNode target = (PathNode)search.Pop();

                if (target != null)
                {
                    foundGoal = hasArrived(target);
                    if (foundGoal)
                    {
                        goalNode = target;
                        lblPath.Text = "PathFound";
                    }
                    else
                    {
                        exploreNode(target);
                    }
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            for (int x = 0; x < numX; x++)
            {
                for (int y = 0; y < numY; y++)
                {
                    grid[x, y].passable = true;
                }
            }

            pictureBox1.Refresh();
        }
    }
}
