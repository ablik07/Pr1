using System;
using System.Collections.Generic;

public abstract class Person
{
    private int id;
    private string name;
    private int age;
    private string email;

    protected Person(int id, string name, int age, string email)
    {
        this.id = id;
        this.name = name;
        this.age = age;
        this.email = email;
    }

    public int Id => id;
    public string Name => name;
    public int Age => age;
    public string Email => email;

    public abstract string GetRole();

    public virtual string GetInfo()
    {
        return $"ID: {id}, Имя: {name}, Возраст: {age}"
    }
}

 