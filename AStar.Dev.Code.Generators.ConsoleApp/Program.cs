// See https://aka.ms/new-console-template for more information

using AStar.Dev.Code.Generators.ConsoleApp.Models;

Console.WriteLine("Hello, World!");

var createCustomerCommand = new CreateCustomerCommand
                            {
                                Name          = "Jay",
                                Email         = "mind@yourown.business",
                                PhoneNumber   = "1234567890",
                                City          = "Nowhere",
                                Country       = "NotExisting",
                                PostOrZipCode = "1234567890",
                                Region        = "1234567890",
                            };

Console.WriteLine(createCustomerCommand.ToString());
