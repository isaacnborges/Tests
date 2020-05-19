﻿using System;
using System.Collections.Generic;
using System.Linq;
using Bogus;
using Bogus.DataSets;
using Features.Clientes;
using Moq.AutoMock;
using Xunit;

namespace Features.Tests
{
    [CollectionDefinition(nameof(ClienteAutoMockerCollection))]
    public class ClienteAutoMockerCollection : ICollectionFixture<ClienteTestsAutoMockerFixture>
    {
    }

    public class ClienteTestsAutoMockerFixture : IDisposable
    {
        public CustomerService ClienteService;
        public AutoMocker Mocker;

        public Customer GerarClienteValido()
        {
            return GerarClientes(1, true).FirstOrDefault();
        }

        public IEnumerable<Customer> ObterClientesVariados()
        {
            var clientes = new List<Customer>();

            clientes.AddRange(GerarClientes(50, true).ToList());
            clientes.AddRange(GerarClientes(50, false).ToList());

            return clientes;
        }

        public IEnumerable<Customer> GerarClientes(int quantidade, bool ativo)
        {
            var genero = new Faker().PickRandom<Name.Gender>();

            //var email = new Faker().Internet.Email("eduardo","pires","gmail");
            //var clientefaker = new Faker<Cliente>();
            //clientefaker.RuleFor(c => c.Nome, (f, c) => f.Name.FirstName());

            var clientes = new Faker<Customer>("pt_BR")
                .CustomInstantiator(f => new Customer(
                    Guid.NewGuid(),
                    f.Name.FirstName(genero),
                    f.Name.LastName(genero),
                    f.Date.Past(80, DateTime.Now.AddYears(-18)),
                    "",
                    ativo,
                    DateTime.Now))
                .RuleFor(c => c.Email, (f, c) =>
                      f.Internet.Email(c.Name.ToLower(), c.Lastname.ToLower()));

            return clientes.Generate(quantidade);
        }

        public Customer GerarClienteInvalido()
        {
            var genero = new Faker().PickRandom<Name.Gender>();

            var cliente = new Faker<Customer>("pt_BR")
                .CustomInstantiator(f => new Customer(
                    Guid.NewGuid(),
                    f.Name.FirstName(genero),
                    f.Name.LastName(genero),
                    f.Date.Past(1, DateTime.Now.AddYears(1)),
                    "",
                    false,
                    DateTime.Now));

            return cliente;
        }

        public CustomerService ObterClienteService()
        {
            Mocker = new AutoMocker();
            ClienteService = Mocker.CreateInstance<CustomerService>();

            return ClienteService;
        }

        public void Dispose()
        {
        }
    }
}