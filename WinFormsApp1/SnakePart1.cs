using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using System.Text;
//using UsedDrawControl = System.Windows.Forms.PictureBox;  //задать контрол, который будет исп-ся для рисования
using UsedDrawControl = System.Windows.Forms.Label;         //задать контрол, который будет исп-ся для рисования

namespace Snake {

    /* ОПИСАНИЕ ЛОГИКИ РАБОТЫ ИГРЫ
     * ИНИЦИАЛИЗАЦИЯ ПЕРЕМЕННЫХ, КОНСТАНТ
        -во 2й части частичного класса змейки:
          -добавить enum-тип
          -дописать массив пересечения с другими элементами
        -поготовка необходимых констант, битовых карт внешнего вида
        -создание массива для элементов управления, которые будут расположены на карте
        -объявление объектных переменных элементов игры с обязательным вызовом конструктора класса!
          например, internal Fruit fruit = new(); 
          при этом, в словарь controlValue заносится имя контролов и начальные значения

     * Вызов КОНСТРУКТОРА ФОРМЫ
        -добавление элементов на форму:
          -занесение исходных значений TextBox-ов в словарь controlValue <string, int>
          -для всех элементов списка controlValue вызов AddControl() для автоматического добавления Label + TextBox на форму
            -назначение обработчика события при изменении значений с целью изменения цвета текста TextBox
            -назначение обработчика события нажатия ENTER
        -прорисовка карты
        -запуск ожидания нажатия клавиши курсора

     * ОБРАБОТЧИК СОБЫТИЯ НАЖАТИЯ КЛАВИШ КУРСОРА
        -задание константам dirX, dirY соотв-щих значений

     * ОБРАБОТЧИК СОБЫТИЙ ТАЙМЕРА
        -проверка выхода за границы
        -потушить проглоченное яблоко, дошедшее до конца хвоста
        -получить список элементов в новой позиции головы
        -прверка пересечения с другим предметом...
        -проверка укуса саму себя
        -проверка поедания фрукта
        -переместить хвост на место новой позиции головы
        -увеличить счётчик, указывающий на хвост змейки

     * КОНЕЦ ИГРЫ -> ГЕНЕРАЦИЯ НОВОЙ КАРТЫ
        -чтение новых значений параметров с формы...
        -очистить все массивы с элементами управления на карте...
        -расположение элементов игры...
        -разрешить запук игры по нажатию клавиш курсура
    */

    public enum GI { Form };

    public partial class Snake {

        #region СЛУЖЕБНЫЕ ПАРАМЕТРЫ ОБЕСПЕЧЕНИЯ РАБОТОСПОСОБНОСТИ
          private static int currMapWidth = -1;        //кол-во линий должно быть на 1 больше, чем кол-во клеток 
          private static int currMapHeight = -1;
    
          internal static int score;                //длина змейки
          private static int headX, headY;         //позиция головы змейки
          internal static int tail = 0;             //позиция элемента в массиве змейки, указывающего на хвост
          internal static int dirX, dirY;          //коеф. умножения (-1, +1), задающие направление движения
          internal static bool startFlag = false;  //флаг начала игры
        
        //структура для анализа элементов в позиции XY
          internal ElementsAtXY elementsAtXY = new();

          private static BitPosition[,] allocatedElements;
          internal static UsedDrawControl[] snakeArr= new UsedDrawControl[100];         //массив элементов змейки
          private static UsedDrawControl[] linesX  = new UsedDrawControl[200];         //массивы линий
          private static UsedDrawControl[] linesY  = new UsedDrawControl[200];         //mbsim may be simplified

        //размеры элементов управления
          private static int newControl_Ypos;            //позиция Y для добавления очередного элемента управления на форму
          private static int newControl_Xpos;            //позиция Y для добавления очередного элемента управления на форму
          private static int newCntrol_LabelWidth = 70;  //ширина подписи
          private static int newCntrol_LabelHeight= 20;  //ширина подписи
          private static int newCntrol_TextBoxWidth;     //ширина поля ввода (расчит-ся автомат)
        //треб для изменения цвета на форме при изменении значения (хранит текущее значение)
          /// <summary>словарь: имя элемента управления, значение(текст)</summary>
          internal static Dictionary<string, int> controlValue = new();   
          
          internal static Timer timer = new Timer();

          internal delegate void DHeadOver();
          internal event DHeadOver HeadOver;
        #endregion

        /*------------------------------------------------------------------------------------------------------------------*/
        #region ВСПОМОГАТЕЛЬНЫЕ ПРОЦЕДУРЫ (не влияющие на логику работы) 

          /*==================================================================================================================*/
            internal struct ElementsAtXY {
                //список элементов в заданном знакоместе
                  public Dictionary<ElementId, AElement> list;

                //флаги уст элем-тов в заданном знакоместе
                  private int bits;
                  public  int Bits { get=>bits; }

                //координаты элемента в массиве карты
                  private Point xy;

                //индексатор, доступ к битам установленнх элементов
                  public bool this [BitPosition index, BitPosition index2 = BitPosition.EmptyField] {
                      get {
                          bool res=(bits & (int)index) != 0;
                          if (index2 != BitPosition.EmptyField) {
                            res |= (bits & (int)index2) != 0;
                          }
                          return res;
                      }
                      set {
                          if (value) {
                            //установка бита
                              bits |= (int)index; }
                          else {
                            //сброс бита
                              bits &= ~((int)index);
                          }
                        //сохранить изменённое значение
                          allocatedElements[xy.Y, xy.X] = (BitPosition)bits;
                      }
                  }

                /*------------------------------------------------------------------------------------------------------------------*/
                /*Получить список объектов в позиции X,Y */                 
                  public void Read(Point loc) {    //private IEnumerable<Elements> ElementsAtXY(Point loc) { 
                    //вместо конструктора
                      list ??= new Dictionary<ElementId, AElement>();
                      xy = loc;

                    //подготовка к создание нового списка элементов
                      list.Clear();

                    //получить элемент(ы), занимающие позицию X,Y
                      bits = (int)allocatedElements[loc.Y,loc.X];
                      int bits_tmp = bits;

                    //пройтись по этим элементам в данной точке..
                      int j=1;    //очередной элемент в точке XY
                      int k=1;    //располагать его выше/ниже
                      int i=0;
                      while (bits_tmp>0) {
                        //i - перебирает элементы от 0 и дальше..
                        //делим value на 2, смотрим остаток
                        //если остаток есть, значит i-элемент размещён в данн позиции
                          int a = bits_tmp % 2;
                          if (a>0) {
                            //если остаток есть, значит бит установлен
                              //добавить в словарь класс элемента, находящегося в XY
                              list.Add((ElementId)i, elements[(ElementId)i]);
                          }
                          bits_tmp /= 2;
                          i+=1;
                      }
  			          }
                /*------------------------------------------------------------------------------------------------------------------*/
                /*Проверить,разрешено ли добавление элемента в позицию XY */
                  public bool IsOverlapAllowed(ElementId element) {
                    //пройтись по этим элементам в данной точке..
                      if (list == null) return true;
                      foreach (var item in list) {
                        //проверить по справоч таблице, разрешается ли элементу перекрытие с заданым
                          if (allowOverlap[(int)item.Value.id, (int)element] == 0) {
                              return false;
                          }
                      }
				              return true;
			            }
                /*------------------------------------------------------------------------------------------------------------------*/
            }
          /*==================================================================================================================*/

          /*------------------------------------------------------------------------------------------------------------------*/
            /*Добавление эл.управл. на форму */
            internal static int AddControl(string name, int value) {
                string name_ = name.Replace(" ", "");
              //добавление метки
                Label lb = new Label { 
                    Name = "lbl" + name_,
                    Width = newCntrol_LabelWidth,
                    Height= newCntrol_LabelHeight,
                    Text = name,
                    AutoSize = false,
                    TextAlign = ContentAlignment.MiddleLeft,
                    Location = new Point(newControl_Xpos, newControl_Ypos)    
                };
                form.Controls["grParams"].Controls.Add(lb);
              //добавление поля ввода
                TextBox tb = new TextBox { 
                    Name = "tb" + name_,
                    Width = newCntrol_TextBoxWidth,
                    Height= newCntrol_LabelHeight,
                    Text = value.ToString(),
                    AutoSize = false,
                    TextAlign =  HorizontalAlignment.Left,
                    Location = new Point(newControl_Xpos + newCntrol_LabelWidth - 5, newControl_Ypos)
                };
                form.Controls["grParams"].Controls.Add(tb);
                tb.BringToFront();

              //добавить в словарь начальное значение элемента
                controlValue.Add (name_, value);

              //назначение обработчика события при измен значения
                tb.TextChanged += control_TextChanged;
                tb.KeyPress += control_KeyPress;

                newControl_Ypos += tb.Height + 1;
                return 0;
            }
          /*------------------------------------------------------------------------------------------------------------------*/
          
          /*------------------------------------------------------------------------------------------------------------------*/
            /*Чтение значения с формы */
            /// <summary>читает значение с формы и обновляет значение в словаре</summary>
            /// <param name="controlName">имя элемента управления</param>
            /// <returns>true, если получено новое значение; false,если получено то же самое значение; null, если преобразовать в int не удалось</returns>
            static private bool? GetFormValue(string controlName) {
              //true, если получено новое значение
              //false,если получено то же самое значение
              //null, если преобразовать в int не удалось
                try {
                  //элемент управления на форме
                    var сontrol = form.Controls["grParams"].Controls["tb"+controlName];
                  //превратить текстовое значение элемента упраления в число
                    int tmpValue = Convert.ToInt32(form.Controls["grParams"].Controls["tb"+controlName].Text); 

                  //если предыдущее значение элемента управления не равно новому
                    if (controlValue[controlName] != tmpValue) { 
                      //обновить значение в словаре
                        controlValue[controlName] = tmpValue;
                        return true;
                    }
                    else {
                      return false; 
                    }
                }    
                catch { return null; }
            }
          /*------------------------------------------------------------------------------------------------------------------*/

          /*------------------------------------------------------------------------------------------------------------------*/
            /*прорисовка сетки*/ 
            static void DrawField() {

              //записать в свойства значения из словаря
                mapWidth = controlValue["MapWidth"];
                mapHeight = controlValue["MapHeight"];
                brickSize = controlValue["BrickSize"];

              //обновление размера формы
                form.Width  = (MapWidth) * BrickSize + ExtFieldWidth + 15;
                form.Height = (MapHeight + 1)* BrickSize + 25;

              //удалить лишние горизонт и верт линии
                //если есть лишние линии, то currMapHeight будет больше MapHeight
                //цикл в сторону уменьшения, удаляем последние линии
                for (int i = currMapHeight; i > MapHeight; i--) {
                  form.Controls.Remove(linesX[i]);
                  linesX[i] = null;
                }
                for (int i = currMapWidth; i > MapWidth; i--) {
                  form.Controls.Remove(linesY[i]);
                  linesY[i] = null;
                }

              //добавить недостающие горизонт линии
                for (int i = MapHeight; i > currMapHeight; i--) {
                    UsedDrawControl fieldGrid = new UsedDrawControl {
                      BackColor = Color.Black,
                      Size = new Size(1, 1),
                      Location = new Point(0, 0)
                    };
                    form.Controls.Add(fieldGrid);
                    linesX[i] = fieldGrid;
                }
              //добавить недостающие верт линии
                for (int i = MapWidth; i > currMapWidth; i--) {
                    UsedDrawControl FieldGrid = new UsedDrawControl {
                      BackColor = Color.Black,
                      Size = new Size(1,1),
                      Location = new Point(0,0)
                    };
                    form.Controls.Add(FieldGrid);
                    linesY[i] = FieldGrid;
                }
                currMapWidth = MapWidth;
                currMapHeight = MapHeight;

              //правим размеры горизонт линии
                for (int i = 0; i <= currMapHeight; i++) {
                  linesX[i].Size = new Size(currMapWidth * BrickSize, 1);
                  linesX[i].Location = new Point(0, i * BrickSize);
                }
              //правим размеры вертик линии
                for (int i = 0; i <= currMapWidth; i++) {
                  linesY[i].Size = new Size(1, currMapHeight * BrickSize);
                  linesY[i].Location = new Point(i * BrickSize, 0);
                } 
                  
            }
          /*------------------------------------------------------------------------------------------------------------------*/

          /*------------------------------------------------------------------------------------------------------------------*/
            /*обработчик событий нажатия клавиш при изменении значений параметров игры*/
            private static void control_TextChanged(object sender, EventArgs e) { /*
                -если значение в элементе управления равно значению соотв. свойства
                  -изменить цвет контрола
                  -иначе - восст. цвет контрола
                */
                string parameterName= (sender as TextBox).Name;
                int.TryParse(((TextBox)sender).Text, out int tmpValue);
                if (controlValue[parameterName] != tmpValue) {
                    ((TextBox)sender).ForeColor = TextBoxCnanged; }
                else {
                    ((TextBox)sender).ForeColor = TextBoxApplied; 
                }
            }
          /*------------------------------------------------------------------------------------------------------------------*/
          
          /*------------------------------------------------------------------------------------------------------------------*/
            /*Применение изменений при нажатии ENTER*/
            private static void control_KeyPress(object sender, KeyPressEventArgs e) {
                if (e.KeyChar == (char)Keys.Enter) {
                    GenerateMap();
                }
            }
          /*------------------------------------------------------------------------------------------------------------------*/

          /*------------------------------------------------------------------------------------------------------------------*/
          /*разместить элемент на карте *//*
          -начать счёт кол-ва попыток
          -взять случайную координату
          -проверка возможности размещения всех точек объекта
          >если хоть одна точка не м.б. размещена
            взять другу коорд всего объекта и повторить до кол-ва попыток
          >если все точки могут быть размещены
            -разместить каждую точку объекта
              создать элемент управления
              занести eго в массив элементов соответствующего типа
              добавить в массив карты размещение объекта
          */
            internal static bool AllocateElement(ElementId element, int cnt, UsedDrawControl[] arr, ref int arr_len, Color color, int[,] map) { 
              //служебные переменные
                Point loc_obj;              //лев верх коорд всего объекта
				        Point loc_i = new Point();  //коорд очередной его точки
				        Random r = new Random();
                ElementsAtXY elementsAtXY=new();
              //определить размерности массива, задающего макет элемента
				        int map_height = map.GetUpperBound(0);
				        int map_width  = map.GetUpperBound(1);

				      //цикл по кол-ву создаваемых элементов
				        for (int i = 0; i < cnt; i++) {
						        int  ok_obj=0;	  //счётчик неудачного размещения
						        bool ok_i;        //успешное размещение очередной точки объекта
						    
                 NextObjLocation: 
							    //случайная координата лев. верхнего угла размещаемого объекта
							      #if TEST
                      loc_obj = CustomPoint;
                    #else
                      loc_obj = new Point(r.Next(0, currMapWidth - 1 - map_width), r.Next(0, currMapHeight - 1 - map_height));
                    #endif
                    
                  //проверка возможности размещения всего объекта в данном месте
								    for (int y = 0; y <= map_height; y++) {
										    for (int x = 0; x <= map_width; x++) {
											    //если точка не отображается, взять следующую координату
												    if (map[y, x] == 0) continue;     //ask ошибка при F11
                          //координата очередной размещаемой точки
												    loc_i.X = loc_obj.X + x;
												    loc_i.Y = loc_obj.Y + y;
											    //проверка, разрешено ли резмещение точки в этом месте
                            elementsAtXY.Read(loc_i);
												    ok_i = elementsAtXY.IsOverlapAllowed(element);
												    if (!ok_i ) {
                                ok_obj += 1;
						                  //если все попытки вышли - неуспешный выход 
						                    if (ok_obj == 20) return false;
                                goto NextObjLocation;	  //взять брать слудующую начальную координату всего объекта
                            }
										    }
								    }
                  //размещение объекта в данном месте
								    for (int y = 0; y <= map_height; y++) {
										    for (int x = 0; x <= map_width; x++) {
											    //если точка не отображается, взять следующую координату
												    if (map[y, x] == 0) continue;    

                          //размещение элемента в массиве карты
                            allocatedElements[loc_obj.Y + y, loc_obj.X + x] |= (BitPosition)Math.Pow(2,(int)element);

											    //разместить элемент в своём массиве и на экране
											      //координаты на экране
                            loc_i.X = (loc_obj.X + x) * BrickSize;
												    loc_i.Y = (loc_obj.Y + y) * BrickSize;

                            arr[arr_len] = new UsedDrawControl {
												      Location = loc_i,
												      Size = new Size(BrickSize, BrickSize),
												      BackColor = color,
                              TextAlign = ContentAlignment.TopLeft,
                            };
												    form.Controls.Add(arr[arr_len]);
												    arr_len += 1;
										    }
								    }
				        }
				       return true;
			      }
          /*------------------------------------------------------------------------------------------------------------------*/

        #endregion 

        /*------------------------------------------------------------------------------------------------------------------*/
        /*КОНСТРУКТОР КЛАССА *//*
          -создать форму и инициализировать статическую ссылку на неё
          -вычислить и задать размер формы
          -назначание обработчика события нажатия клавиши
          -подгот нач знач служеб переменных для расположения элементов управления
          -расположить основноые элементы управления
             -добавить в словарь их нач значения
          -расположить элементы управления других элементов игры
             -добавить в словарь их нач значения
          -генерация сетки, карты
        -----------------------------------------------------------*/
          public Snake() { 
              //если режим гафики - элементы управления
              if (gi == GI.Form) {
                //создать форму и получить ссылку на неё
                  snake = this;
                  form = new Form1();
                //вычисление и задание размера формы
                  form.Width  = (MapWidth) * BrickSize + ExtFieldWidth + 15;
                  form.Height = (MapHeight + 1)* BrickSize + 25;

                //обработчки событий формы
                  form.KeyDown += Form1_KeyDown;

                //подготовиться к размещению доп. элементов
                  var btn = form.Controls["grParams"].Controls["btnGenerateMap"];
                  newControl_Xpos = btn.Location.X;
                  newControl_Ypos = btn.Location.Y+btn.Height + 15;
                  //newCntrol_LabelWidth  = btn.Width;
                  newCntrol_TextBoxWidth= ExtFieldWidth-newCntrol_LabelWidth-5;
                
                //добавление на форму общих элементов управления с занесением их значений в словарь
                  AddControl("Map Width", MapWidth);
                  AddControl("Map Height", MapHeight);
                  AddControl("Brick Size", BrickSize);
                  AddControl("Speed", SnakeSpeed);

                /*ввести остальные элементы в игру:
                  -вызвать конструктор класса элемента
                    -разместит его основной TextBox на форме
                    -занесёт нач знач в словарь
                  -добавить класс элемента в список элементов (словрь, key=тип элемнта, val=class)  */
                  elements.Add(ElementId.Fruit, new Fruit(ElementId.Fruit, "Fruit cnt", 1, 1, Color.Red));

                //вызов процедур размещения других элементов управления
                  foreach (var item in elements) {
                      item.Value.AddControl();
                  }
              }
              timer.Tick += Timer_Tick;

              DrawField();
              GenerateMap();
          }
        /*------------------------------------------------------------------------------------------------------------------*/

        /*------------------------------------------------------------------------------------------------------------------*/
        //•ОБРАБОТЧИК НАЖАТИЯ КЛАВИШ КУРСОРА
			    private void Form1_KeyDown(object sender, KeyEventArgs e) { /*
            -задание направления движения змейки
            -если это первое нажатие
              -запуск таймера
            -----------------------------------------------------------*/
	        switch (e.KeyCode) {
                case Keys.Escape: 
                  break;
						    case Keys.Left: dirX= -1; dirY = 0;  goto aa;
						    case Keys.Right: dirX = 1; dirY = 0; goto aa;
						    case Keys.Up: dirY = -1; dirX = 0;   goto aa;
						    case Keys.Down: dirY = 1; dirX = 0;  
              aa: 
                if (startFlag) { 
                    timer.Start(); 
                    startFlag = false; 
                    //обнуление счётчика очков
                    form.Controls["grParams"].Controls["lblScore"].Text = "0";
                } 
                if (!timer.Enabled) {
                    timer.Start();
                }
                break;
				     }
			    }
        /*------------------------------------------------------------------------------------------------------------------*/

        /*------------------------------------------------------------------------------------------------------------------*/
        /*ОБРАБОТЧИК СОБЫТИЙ ТАЙМЕРА */ /*
          -вычислить координаты новой позиции головы
          -проверка выхода за ганицы новых координат хвоста (головы)
          -перемещение хвоста змейки на новую позицию головы
          -анализ ячейки нового расположения головы
            >поедание фрукта
            >вызов других процедур
          -----------------------------------------------------*/
          private void Timer_Tick(object sender, EventArgs e) {
              Label tmpLabel=null;

            //координаты хвоста
				      int tailX = snakeArr[tail].Location.X / brickSize; 
              int tailY = snakeArr[tail].Location.Y / brickSize;
        	  //новые координаты головы при движении
              headX += dirX;  headY += dirY;
				      Point loc_MapArr = new Point(headX, headY);
				      Point loc_Screen = new Point(headX * BrickSize, headY * BrickSize);

				    //•ПРОВЕРКА ВЫХОДА ЗА ГРАНИЦЫ
				      if (headX < 0 || headX == currMapWidth || headY < 0 || headY == currMapHeight) {
						      timer.Stop();
						      MessageBox.Show("Вы проиграли!");
						      GenerateMap();
						      return;
				      }

				    //ПОТУШИТЬ ПРОГЛОЧЕННОЕ ЯБЛОКО, КОТОРОЕ ДОШЛО ДО КОНЦА ХВОСТА
				      if (snakeArr[tail].Width != BrickSize) {
                //восстановить размеры
						      snakeArr[tail].Width = BrickSize;
						      snakeArr[tail].Height = BrickSize;
						      snakeArr[tail].BackColor = SnakeColor;
						      snakeArr[tail].SendToBack();
                //очистить бит "проглоченное яблоко"
                  allocatedElements[tailY, tailX] &= ~BitPosition.EatenFruit;
				      }

            //список элементов в новой позиции головы
              elementsAtXY.Read(loc_MapArr);

              #if TEST
              //перечитать содержимое элементов в месте головы
                elementsAtXY.Read(loc_MapArr);
              #endif

            //•ПРОВЕРКА ПЕРЕСЕЧЕНИЯ С ДРУГИМ ПРЕДМЕТОМ
              //if (elementsAtXY.list.Count > 0) {
              if (elementsAtXY.Bits > 0) {

				        //ПРОВЕРКА УКУСА САМУ СЕБЯ
                  //if (elementsAtXY.list.Contains(Elements.Snake)) {
                  if (elementsAtXY[BitPosition.Snake, BitPosition.EatenFruit]) {
						          timer.Stop();
						          MessageBox.Show("Вы проиграли!");
						          GenerateMap();
						          return;
				          }

				        //ПРОВЕРКА ПОЕДАНИЯ ФРУКТА
                  if (elementsAtXY[BitPosition.Fruit]) {
				            //if (elementsAtXY.list.Contains(Elements.Fruit)) {

                    /*ЛОГИКА РАБОТЫ*/ {/*
                    * крутится счётчик tail, указывающий какой из элементов массива в данный момент является самым старым (хвостом) и требует перемещения. 
                    * До этого момента голова находилась в элементе snake[tail-1], сейчас ей надо переместить в [tail]
                    * и так, змейка наезжает на фрукт. В этот момент:
                    * - увеличился размер массива змейки (score++);
                    * - на этом новом месте нужно разместить бывший фрукт, который превращается в элемент змейки;
                    * - координаты хвоста приравнять к координатам фрукта, можно увеличить его в размере;
                    * Когда tail станет равен score - это будет момент визуального увеличения змейки () и один из двух элементов, находящиеся на одном месте, переместится в изголовье.
						        */}

                    //ПРЕВРАТИТЬ ФРУКТ В ЗМЕЙКУ
                      //именно хвост превращаем в проглоченный фрукт на месте головы (перекрывая проглоченный фрукт)
                        //SnakeArr[tail].Location = loc_Screen;   //переместится позже
						            snakeArr[tail].BackColor = EatenFruitColor;
						            snakeArr[tail].Width += BrickSize / 3;
						            snakeArr[tail].Height += BrickSize / 3;
						            snakeArr[tail].Left -= BrickSize / 6;
						            snakeArr[tail].Top -= BrickSize / 6;
						            snakeArr[tail].BringToFront();

                      //увеличение массива элементов змейки
                        score++;
                        form.Controls["grParams"].Controls["lblScore"].Text = score.ToString();
                      //в новом месте массива змейки (score) разместить бывший фрукт
                        snakeArr[score] = elements[ElementId.Fruit].bricksArr[0];
						            elements[ElementId.Fruit].count--;
                      //фрукт покрасить в проглоченный фрукт
						            elements[ElementId.Fruit].bricksArr[0].BackColor = SnakeColor;
                        elementsAtXY[BitPosition.Fruit] = false;
                        elementsAtXY[BitPosition.EatenFruit] = true;
                        //allocatedElements[headY, headX] ^= (MapElement)Math.Pow(2,(int)Elements.Fruit);   //ask
                        //allocatedElements[headY, headX] |= (MapElement)Math.Pow(2,(int)Elements.Snake);

                      
                  }
              }

            //вызвать метод HeadOver() для элементов, находящихся в новой позиции головы
              foreach(var item in elementsAtXY.list) {
                  item.Value.HeadOver();
              }

				    //•ПЕРЕМЕСТИТЬ ХВОСТ ВПЕРЁД, УВЕЛИЧИТЬ КОНСТНТУ tail
				      //переместить хвост на место новой позиции головы
				          snakeArr[tail].Location = loc_Screen;
                  snakeArr[tail].BringToFront();
                //отображение изменений в массиве карты
                  allocatedElements[tailY, tailX] &= ~BitPosition.Snake; //ask
                  allocatedElements[headY, headX] |=  BitPosition.Snake;

                #if TEST
                //если в режиме тестирования перемещённый хвост был перекрыт и подсвечивался меткой
                  //удалить эту метку (на том месте остался только один неперекрытый элемент)
                    if (overlapedIDDict.ContainsKey(tail)) {
                       form.Controls.Remove(overlapedIDDict[tail]);
                       overlapedIDDict.Remove(tail);
                    }
                #endif

				        //крутим счётчик, указывающий на хвост змейки
						      if (tail == score) {
                     tail = 0; }
                  else {
                     tail++;
                  }

                #if TEST
                //обозначить меткой положение хвоста
                  lbl_t.Location = new Point (snakeArr[tail].Location.X, snakeArr[tail].Location.Y+snakeArr[tail].Size.Height-testLabelSize.Height);
                  lbl_t.Text = tail.ToString();
                  lbl_t.BringToFront();
                  //elementsAtXY.Read(loc_MapArr);
                  
                  tmpLabel?.BringToFront();
                #endif 
          }
        /*------------------------------------------------------------------------------------------------------------------*/
        
        /*------------------------------------------------------------------------------------------------------------------*/
        /*ГЕНЕРАЦИЯ КАРТЫ *//*
          -чтение новых значений параметров с формы
          -если были изменения в размерах поля
            -вычисление и задание новых размеров формы
            -прорисовка сетки поля
          -расположение элементов игры
          -вызов процедур расположения элементов игры 
          */
          public static bool GenerateMap() { 
            
            /*----- ЛОКАЛЬНЫЕ ПРОЦЕДУРЫ ----------------------------------------------------------------------------------------*/
            //очистить переданый массив
			        void ClearObject() {

                //пройти по всем элементам игры
                  foreach (var el in elements) {

                    //очистить массив расположенных на карте объектов
                      int i = el.Value.count;
                      while (i-- > 0) {
							            form.Controls.Remove(el.Value.bricksArr[i]);
							            el.Value.bricksArr[i] = null;
					            }
					            el.Value.count = 0;
                  }

			        }
            /*------------------------------------------------------------------------------------------------------------------*/

              //ЧТЕНИЕ НОВЫХ ЗНАЧЕНИЙ ПАРАМЕТРОВ С ФОРМЫ
                //чтение общих параметров игры
                  bool? val1 = GetFormValue("MapWidth");
                  bool? val2 = GetFormValue("MapHeight");
                  bool? val3 = GetFormValue("Bricksize");
                  val1 = val1 | val2 | val3;

                  if (val1 != null & val1 == true) {
                     DrawField();
                  }

                  GetFormValue("Speed");
                  snakeSpeed = controlValue["Speed"];
                  timer.Interval = snakeSpeed;

                //чтение значенией из TextBox-ов
                  foreach (var item in controlValue) {
                     GetFormValue(item.Key);
                  }
                //вызов процедур получения значений с формы остальных элементов управления (не TextBox)
                  foreach (var item in elements) {
                     item.Value.ReadControlVlaue();
                  }

              /*РАСПОЛОЖЕНИЕ ЭЛЕМЕНТОВ ИГРЫ*/ {
                //создадим отдельную локальну структуру для размещения элемента
                  allocatedElements = new BitPosition[MapHeight, MapWidth];

                //очистить все массивы
                  ClearObject();

                //РАСПОЛОЖИТЬ ВСЕ ЭЛЕМЕНТЫ ИГРЫ
                  foreach (var item in elements.Values) {
                      item.AllocateElement();
                  }

                //расположить змейку
                  #if TEST
                    CustomPoint = new Point (0, 0);
                  #endif
                  if (!AllocateElement(ElementId.Snake, 1, snakeArr, ref score, SnakeColor, onePointMap)) { 
                      MessageBox.Show("Не удаётся разместить змейку на карте. Измените параметры"); 
                      return false; 
                  }
                  tail = 0; score = 0;
                  headX = snakeArr[0].Location.X / brickSize;
                  headY = snakeArr[0].Location.Y / brickSize;

                  #if TEST 
                  //отобразить номер первого элемента массива snakeArr
                    snakeArr[0].Text = score.ToString();
                    snakeArr[0].Font = new Font ("Arial", 6);
                    snakeArr[0].BringToFront();
                  #endif

                  /*создание метки указателя на хвост*/ {
                    #if TEST
                      lbl_t ??= new Label { 
                          Name = "lbl_t",
                          Size=testLabelSize,
                          Text = "0",
                          AutoSize = false,
                          TextAlign = ContentAlignment.MiddleCenter,
                          Location = new Point (snakeArr[0].Location.X, snakeArr[0].Location.Y+snakeArr[0].Size.Height-testLabelSize.Height)
                      };
                      lbl_t.Font = new Font("Arial", 6);
                      form.Controls.Add(lbl_t);
                      form.Controls["lbl_t"].BringToFront();
                    #endif
                  }
              }

            //разрешение запустить игру(таймер) по нажатию клавиш курсура
              startFlag = true;

            //убрать курсор со всех полей ввода
              form.Controls["GrParams"].Controls["btnGenerateMap"].Focus();

              return true;
          }
        /*------------------------------------------------------------------------------------------------------------------*/

    }




}
