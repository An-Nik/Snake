﻿
namespace Snake {
    internal interface IElement {

     /* ЧТОБЫ ВВЕСТИ НОВЫЙ ЭЛЕМЕНТ, НУЖНО:
     --- ПОДГОТОВКА ---------------------------
      -создать Класс:IElement
      -придумать имя элемента, которое будет и:
        -имя textbox на форме
        -имя параметра (содержимое textbox) в словаре
      -объявить массив элементов, которые будут представлять элемент на форме

     --- В ЧАСТИЧНОМ КЛАССЕ ЗМЕЙКИ ДОБАВИТЬ ---
      -соответствующую константу enum
      -создать его битовую карту внешнего вида (одну или несколько)
      -заполнить массив пересечения с другими элементами

     --- КОНСТРУКТОР КЛАССА -------------------
      -добавить имя элемента в словарь с его нач. значением
      -зарегистрировать элемент (вызвать метод Snake.RegistrateElement(this))

     --- СОБЫТИЯ ------------------------------
      HeadOver()

     */

      //добавить элемент управления на форму с начальным значением
        void  AddControl();

      //прочитать значение элемента управления
        void ReadControlVlaue();

      //расположить элемент на карте
        bool AllocateElement();

      //пересечение с головой змейки
        void HeadOver();
    }
}