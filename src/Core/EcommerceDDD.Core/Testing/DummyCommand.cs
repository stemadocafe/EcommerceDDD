﻿using EcommerceDDD.Core.CQRS.CommandHandling;

namespace EcommerceDDD.Core.Testing;

public record class DummyCommand(DummyAggregateId Id) : ICommand {}