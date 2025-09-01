using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;

using System.Text;
using System.Windows.Forms;

namespace Zmija
{
	public partial class Form1 : Form
	{
		#region Fields

		const int columns = 15;
		const int rows = 15;

		const int fieldWidth = 35;
		const int fieldHeight = 35;

		const int startSegments = 5;

		const string title = "Zmija";

		const int foodLimit = 30;

		List<List<CustomPanel>> fields = new List<List<CustomPanel>>();

		Snake snake = null;

		Keys key = Keys.Enter;

		Timer timer = new Timer();
		Timer foodTimer = new Timer();
		Timer levelTimer = new Timer();

		Color foodColor = Color.Red;
		Color fieldColor = Color.White;

		bool spacePressed = false;

		#endregion //Fields

		#region Constructor

		public Form1()
		{
			InitializeComponent();

			CreateFields();

			MapCustomPanels();

			CreateSnake();

			timer.Tick += new EventHandler(timer_Tick);

			timer.Interval = 300;

			timer.Start();

			foodTimer.Tick += new EventHandler(foodTimer_Tick);
			foodTimer.Interval = 6000;
			foodTimer.Start();
						
			this.KeyDown += new KeyEventHandler(Form1_KeyDown);
		}

		#endregion //Constructor

		#region Events

		private void foodTimer_Tick(object sender, EventArgs e)
		{
			CustomPanel currentFood = GetCurrentFood();
			CustomPanel foodForInsert = FoodForInsert();

			if (null != currentFood)
			{
				if (currentFood.IsFood)
				{
					currentFood.IsFood = false;
					currentFood.BackColor = fieldColor;
					foodForInsert.IsFood = true;
					foodForInsert.BackColor = foodColor;
				}
			}
			else
			{
				foodForInsert.IsFood = true;
				
				foodForInsert.BackColor = foodColor;
			}
		}

		private void timer_Tick(object sender, EventArgs e)
		{
			if (key == Keys.Left || key == Keys.Right || key == Keys.Up || key == Keys.Down)
			{
				if (!snake.CollisionDetected)
				{
					snake.MoveSnake(key);
				}
			}
		}

		private void Form1_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyData == Keys.Space)
			{
				if (!spacePressed)
				{
					StopGame();
					spacePressed = true;
				}
				else
				{
					ContinueGame();
					spacePressed = false;
				}
			}

			if (e.KeyData == Keys.Left)
			{
				if (snake.movingDirection != Snake.MovingDirection.Right)
				{
					key = e.KeyData;
				}
			}
			else if (e.KeyData == Keys.Right)
			{
				if (snake.movingDirection != Snake.MovingDirection.Left)
				{
					key = e.KeyData;
				}
			}
			else if (e.KeyData == Keys.Up)
			{
				if (snake.movingDirection != Snake.MovingDirection.Down)
				{
					key = e.KeyData;
				}
			}
			else if (e.KeyData == Keys.Down)
			{
				if (snake.movingDirection != Snake.MovingDirection.Up)
				{
					key = e.KeyData;
				}
			}
		}

		private void snake_ScoreChanged(int score)
		{
			this.Text = title + " - " + score.ToString();

			if (score == foodLimit)
			{
				StopGame();

				DialogResult res = MessageBox.Show("Cestitam pobedili ste u igri!\nZa nastavak pritisnite Y.", "KRAJ", MessageBoxButtons.YesNo);

				if (res == DialogResult.Yes)
				{
					ContinueGame();
				}
			}

			if (snake.Score % 5 == 0 && 0 < snake.Score)
			{
				timer.Interval -= 50;
			}
		}

		private void pokreniNovuIgruToolStripMenuItem_Click(object sender, EventArgs e)
		{
			StartNewGame();
		}

		#endregion //Events

		#region Methods

		private CustomPanel GetCurrentFood()
		{
			CustomPanel retPanel = null;

			foreach (List<CustomPanel> rowpanels in fields)
			{
				foreach (CustomPanel panel in rowpanels)
				{
					if (panel.IsFood)
					{
						retPanel = panel;
						break;
					}
				}
			}

			return retPanel;
		}

		private CustomPanel FoodForInsert()
		{

			CustomPanel rndPanel = GetPanelRND();

			while (SnakeContainPosition(rndPanel))
			{
				rndPanel = GetPanelRND();
			}

			return rndPanel;
		}

		private bool SnakeContainPosition(CustomPanel rndPanel)
		{
			bool retValue = false;

			foreach (SnakeSegment segment in snake.SnakeSegments)
			{
				if (object.ReferenceEquals(segment.currentPanel, rndPanel))
				{
					retValue = true;
					break;
				}
			}

			return retValue;
		}

		private CustomPanel GetPanelRND()
		{
			int row = -1;
			int column = -1;

			Random rnd1 = new Random();
			row = rnd1.Next(0, rows - 1);

			Random rnd2 = new Random();
			column = rnd2.Next(0, columns - 1);

			return fields[row][column];
		}

		private void StartNewGame()
		{
			StopGame();

			this.Text = title; 

			foreach (List<CustomPanel> rowpanels in fields)
			{
				foreach (CustomPanel panel in rowpanels)
				{
					panel.BackColor = fieldColor;
					if (panel.IsFood)
					{
						panel.IsFood = false;
					}
				}
			}

			CreateSnake();
			key = Keys.Enter;
			timer.Interval = 300;

			ContinueGame();
		}

		private void StopGame()
		{
			timer.Stop();
			foodTimer.Stop();
		}

		private void ContinueGame()
		{
			timer.Start();
			foodTimer.Start();
		}

		private void CreateFields()
		{
			for (int i = 0; i < rows;i++ )
			{
				List<CustomPanel> rowList = new List<CustomPanel>();

				for (int j = 0; j < columns;j++ )
				{
					CustomPanel newPanel = new CustomPanel();
					newPanel.X = j;
					newPanel.Y = i;
					newPanel.Width = fieldWidth;
					newPanel.Height = fieldHeight;
					newPanel.BackColor = fieldColor;
					newPanel.Location = new Point(j * newPanel.Width, i * newPanel.Height);
					newPanel.BorderStyle = BorderStyle.None;
					rowList.Add(newPanel);
					panelContent.Controls.Add(newPanel);
				}

				fields.Add(rowList);
			}

			fields[rows / 2][columns / 2].BackColor = foodColor;
			fields[rows / 2][columns / 2].IsFood = true;
		}

		private void MapCustomPanels()
		{
			for (int i = 0; i < rows;i++ )
			{
				for (int j = 0; j < columns; j++)
				{
					CustomPanel custPanel = fields[i][j];

					custPanel.Top = GetTopPanel(i, j);
					custPanel.Bottom = GetBottomPanel(i, j);
					custPanel.Left = GetLeftPanel(i, j);
					custPanel.Right = GetRightPanel(i, j);
				}
			}
		}

		private void CreateSnake()
		{
			snake = new Snake();
			snake.fieldColor = this.fieldColor;

			snake.ScoreChanged += new scoreDelegate(snake_ScoreChanged);

			int index = 1;

			for (int i = startSegments - 1; i >= 0;i-- )
			{
				SnakeSegment newSegment = new SnakeSegment();
				newSegment.currentPanel = fields[0][i];
				newSegment.oldPanel = newSegment.currentPanel;
				snake.SnakeSegments.Add(newSegment);
				index++;
			}

			int incr = 0;
			foreach(SnakeSegment segment in snake.SnakeSegments)
			{
				segment.Child = incr + 1 <= snake.SnakeSegments.Count - 1 ? snake.SnakeSegments[incr + 1] : null;
				segment.Parent = incr - 1 <= snake.SnakeSegments.Count - 1 && incr - 1 >= 0 ? snake.SnakeSegments[incr - 1] : null;
				incr++;
			}

			snake.ShowSnake();
		}

		private CustomPanel GetTopPanel(int row, int column)
		{
			CustomPanel retValue = null;

			int previousRow = row - 1;

			if (0 == row)
			{
				retValue = fields[rows - 1][column];
			}
			else if (previousRow > -1)
			{
				retValue = fields[previousRow][column];
			}

			return retValue;
		}

		private CustomPanel GetBottomPanel(int row, int column)
		{
			CustomPanel retValue = null;

			int nextRow = row + 1;

			if(row == rows - 1)
			{
				retValue = fields[0][column];
			}
			else if (nextRow < fields.Count)
			{
				retValue = fields[nextRow][column];
			}

			return retValue;
		}

		private CustomPanel GetLeftPanel(int row, int column)
		{
			CustomPanel retValue = null;

			int previousColumn = column - 1;

			if(column == 0)
			{
				retValue = fields[row][columns - 1];
			}
			else if (previousColumn > -1)
			{
				retValue = fields[row][previousColumn];
			}

			return retValue;
		}

		private CustomPanel GetRightPanel(int row, int column)
		{
			CustomPanel retValue = null;

			int nextColumn = column + 1;

			if(column == columns - 1)
			{
				retValue = fields[row][0];
			}
			else if (nextColumn < fields[row].Count)
			{
				retValue = fields[row][nextColumn];
			}

			return retValue;
		}

		#endregion //Methods

		private void pomocToolStripMenuItem_Click(object sender, EventArgs e)
		{
			MessageBox.Show("Za pokretanje zmije upotrebljavaju se kursori.\nCilj je pojesti 30 komada hrane.\nPauza je SPACE.\nPuno srece u igranju.");
		}
	}

	public delegate void scoreDelegate(int score);

	#region Classes

	public class Snake
	{
		public List<SnakeSegment> SnakeSegments = new List<SnakeSegment>();

		public int Score = 0;

		public Color snakeColor = Color.SlateBlue;

		public Color fieldColor;

		public event scoreDelegate ScoreChanged;

		public enum MovingDirection
		{
			Stop,
			Left,
			Right,
			Up,
			Down
		}

		public MovingDirection movingDirection = MovingDirection.Stop;

		public bool CollisionDetected = false;

		public void MoveSnake(Keys key)
		{
			SnakeSegment segmentHead = this.GetSnakeHead();

			SnakeSegment segmentTail = this.GetSnakeTail();


			//CustomPanel tempPanel = null;

			if (key == Keys.Right)
			{
				if (movingDirection == MovingDirection.Left)
				{
					return;
				}

				this.RefreshRelations(segmentHead.currentPanel.Right);

				movingDirection = MovingDirection.Right;

				//segmentHead.currentPanel = segmentHead.currentPanel.Right;
			}
			else if(key == Keys.Left)
			{
				if (movingDirection == MovingDirection.Right)
				{
					return;
				}

				this.RefreshRelations(segmentHead.currentPanel.Left);

				movingDirection = MovingDirection.Left;

				//segmentHead.currentPanel = segmentHead.currentPanel.Left;
			}
			else if(key == Keys.Up)
			{
				if (movingDirection == MovingDirection.Down)
				{
					return;
				}

				this.RefreshRelations(segmentHead.currentPanel.Top);

				movingDirection = MovingDirection.Up;

				//segmentHead.currentPanel = segmentHead.currentPanel.Top;
			}
			else if (key == Keys.Down)
			{
				if (movingDirection == MovingDirection.Up)
				{
					return;
				}

				this.RefreshRelations(segmentHead.currentPanel.Bottom);

				movingDirection = MovingDirection.Down;

				//segmentHead.currentPanel = segmentHead.currentPanel.Bottom;
			}

			CollisionDetected = DetectCollision(segmentHead);

			if (CollisionDetected)
			{
				return;
			}

			segmentTail.oldPanel.BackColor = fieldColor;
			segmentHead.currentPanel.BackColor = snakeColor;
			

			bool foodDetected = DetectFood(segmentHead);

			if (foodDetected)
			{
				CollisionDetected = DetectCollision(GetSnakeHead());
			}
		}

		private bool DetectCollision(SnakeSegment segmentHead)
		{
			bool retValue = false;

			for (int i = 3; i < SnakeSegments.Count; i++)
			{
				SnakeSegment segment = SnakeSegments[i];

				if (object.ReferenceEquals(segmentHead.currentPanel, segment.currentPanel))
				{
					segmentHead.currentPanel.BackColor = Color.GreenYellow;
					retValue = true;
					break;
				}
			}

			//SnakeSegment collisionSegment = SnakeSegments.Find(delegate(SnakeSegment segment) 
			//{
			//    bool collDetected = false;
			//    if (object.Equals(segment.currentPanel, segmentHead.currentPanel))
			//    {
			//        collDetected = true;
			//    }
			//    return collDetected;
			//}
			//);

			//if (null != collisionSegment)
			//{
			//    collisionSegment.currentPanel.BackColor = Color.GreenYellow;
			//    retValue = true;
			//}

			return retValue;
		}

		private bool DetectFood(SnakeSegment segmentHead)
		{
			bool retValue = false;

			if (segmentHead.currentPanel.IsFood)
			{
				SnakeSegment segment = new SnakeSegment();

				if (movingDirection == MovingDirection.Down)
				{
					segment.currentPanel = segmentHead.currentPanel.Bottom;
				}
				else if(movingDirection == MovingDirection.Up)
				{
					segment.currentPanel = segmentHead.currentPanel.Top;
				}
				else if(movingDirection == MovingDirection.Left)
				{
					segment.currentPanel = segmentHead.currentPanel.Left;
				}
				else if (movingDirection == MovingDirection.Right)
				{
					segment.currentPanel = segmentHead.currentPanel.Right;
				}

				segment.currentPanel.BackColor = snakeColor;
				segment.Child = segmentHead;
				segmentHead.Parent = segment;

				segment.oldPanel = segment.currentPanel;

				segmentHead.currentPanel.IsFood = false;

				SnakeSegments.Insert(0, segment);

				Score += 1;
				if (null != ScoreChanged)
				{
					ScoreChanged(Score);
				}
			}

			return retValue;
		}

		public SnakeSegment GetSnakeHead()
		{
			SnakeSegment retValue = null;
			
			foreach (SnakeSegment segment in SnakeSegments)
			{
				if (!segment.HasParent)
				{
					retValue = segment;
					break;
				}
			}

			return retValue;
		}

		public SnakeSegment GetSnakeTail()
		{
			SnakeSegment retValue = null;

			foreach (SnakeSegment segment in SnakeSegments)
			{
				if (!segment.HasChild)
				{
					retValue = segment;
					break;
				}
			}

			return retValue;
		}

		public void ShowSnake()
		{
			foreach (SnakeSegment segment in SnakeSegments)
			{
				segment.currentPanel.BackColor = snakeColor;
			}
		}

		public void RefreshRelations(CustomPanel nextPanelForHead)
		{
			for (int i = 0; i < SnakeSegments.Count; i++)
			//for (int i = SnakeSegments.Count - 1; i >= 0; i--)
			{
				SnakeSegment segment = SnakeSegments[i];

				if (segment.HasParent)
				{
					CustomPanel tempPanel = segment.currentPanel;

					segment.oldPanel = tempPanel;
					segment.currentPanel = segment.Parent.oldPanel;
				}
				else
				{
					
					CustomPanel tempPanel = segment.currentPanel;

					segment.oldPanel = tempPanel;
					segment.currentPanel = nextPanelForHead;
				}
			}
		}
	}

	public class SnakeSegment
	{
		public SnakeSegment Parent;
		public SnakeSegment Child;

		public CustomPanel currentPanel = null;
		public CustomPanel oldPanel = null;

		public bool HasParent
		{
			get { return null != Parent; }
		}

		public bool HasChild
		{
			get { return null != Child; }
		}
	}
	
	public class CustomPanel : Panel
	{
		public int X;
		public int Y;

		public CustomPanel Top = null;
		public CustomPanel Bottom = null;
		public CustomPanel Left = null;
		public CustomPanel Right = null;

		public bool IsFood = false;

		public bool HasTop
		{
			get { return Top != null; }
		}

		public bool HasBottom
		{
			get { return Bottom != null; }
		}

		public bool HasLeft
		{
			get { return Left != null; }
		}

		public bool HasRight
		{
			get { return Right != null; }
		}
	}

	#endregion //Classes
}
