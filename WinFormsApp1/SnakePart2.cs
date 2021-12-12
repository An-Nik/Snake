using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using System.Text;
using UsedDrawControl = System.Windows.Forms.Label;

namespace Snake {
    public partial class Snake {

        #region ОТКРЫТЫЕ ПАРАМЕТРЫ ИГРЫ  
          //ПАРАМЕТРЫ ИГРЫ ......................................................
            public GI gi = GI.Form;                     //режим граф. интерфейса
            private static int snakeSpeed  = 100;
            public int SnakeSpeed  { get=> snakeSpeed;  set=> snakeSpeed=value;  }    //скорость змейки

          /*ПАРАМЕТРЫ ИГРОВОГО ПОЛЯ..............................................*/
            //размеры поля
              private static int mapWidth  = 30; 
              private static int mapHeight = 30;
              private static int brickSize = 18;       //размер ячейки
              
              public static int MapWidth  { get => mapWidth;   set => mapWidth  = value; } 
              public static int MapHeight { get => mapHeight;  set => mapHeight = value; } 
              public static int BrickSize  
              {   get => brickSize;  
                  set { 
                      if (value < 5)  {
                         brickSize = 5;}
                      else if (value > MapWidth/10) {
                         brickSize = MapWidth/10; }
                      else {
                         brickSize = value;
                      }
                  }
              }

          /*ПАРАМЕТРЫ ВНЕШНЕГО ВИДА..............................................*/
              public static readonly int ExtFieldWidth = 110;     //размер служебного поля на форме
            //цвета
              public static Color SnakeColor     = Color.Gray;
              public static Color EatenFruitColor= Color.Gray;
              public static Color TextBoxCnanged = Color.Red;
              public static Color TextBoxApplied = SystemColors.WindowText;
          /*--------------------------------------------------------------------*/
        #endregion

        #region СЛУЖЕБНЫЕ ПАРАМЕТРЫ ОБЕСПЕЧЕНИЯ РАБОТОСПОСОБНОСТИ

          internal static Snake snake;              //для возможности вызова служебных методов Snake из других классов
          internal static Form form;                //ask почему form.Controls.Remove(item) form.Controls.Add() требовал static
          public   Form GetForm { get => form; }    //возвращает ссылку на форму
        
          internal static Dictionary<ElementId, AElement> elements = new(); //список классов всех имеющихся элементов

        //перечень элементов игры
          public enum ElementId {
				     Snake = 0,
             Eatenfruit=1,
				     Fruit = 2,
				     Stone = 3,
				     Tree	 = 4,
				     Water = 5,
          }

        //битовая маска элементов, если они занимают одну позицию на поле
          internal enum BitPosition {
				     EmptyField = 0,
				     Snake = 1,
             EatenFruit = 2,
				     Fruit = 4,
				     Stone = 8,
				     Tree	 = 16,
				     Water = 32,
			    }
        
        //карта для отображения элемента, состоящего из одной точки
          internal static int[,] onePointMap = { { 1, 0 }, { 0, 0 } };
        
        //карта разрешения пересечения объектов
          internal static int[,] allowOverlap = {
            /*------ Snake EatnFr Fruit Stone Tree Water */
            /*Snake*/ {0,   0,     0,    0,    1,   1},
            /*EatnFr*/{0,   0,     0,    0,    1,   1},
            /*Fruit*/ {0,   0,     0,    0,    1,   1},
            /*Stone*/ {0,   0,     0,    0,    1,   1},
            /*Tree */ {1,   1,     1,    1,    1,   1},
            /*Water*/ {1,   1,     1,    1,    1,   1},
          };
        
          #if TEST
          //в режиме тестирования для размещения элемента используется CustomPoint вместо Rnd()
            internal static Point CustomPoint;

          //метка на карте указывающая на хвост
            private static UsedDrawControl lbl_t;
            internal static Size testLabelSize = new (8, 8); 
            internal static Dictionary<int,Label> overlapedIDDict = new();
          #endif
        
        #endregion

    }
}
