# Weathertastic-Isolated
Hello potential employers!

This repo is an example of how I tend to write microservices.  The controller has a single Get operation and returns the current temp of the city passed to it. It utilizes Automapper to easily map between Anti-corruption Layers and Polly for Circuit Breaker goodness. Logging is handled by Serilog which is currently just logging to the console. Unit tests were written using XUnit and the APIs were mocked using my boy Richard Szalay's awesome MockHttp library.  No, I don’t know the man personally but if I ever do get the chance to meet him I will buy him a beer. His library has saved my bacon more times than I can count. If you love the service great.  If you hate it, please let me know how I could have done things differently and how I can make it better.  Remember people, if you not refactoring, you’re not doing your job!

