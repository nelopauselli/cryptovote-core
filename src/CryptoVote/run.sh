#!/bin/bash

# para ejecutar en entorno linux
export DOTNET_ROOT=$HOME/dotnet 
export PATH=$PATH:$HOME/dotnet
dotnet CryptoVote.dll Miner:Address=QXuwUXRhANuouMkHTc2tbcZhkcucShZQKEhs7XSfseSd6Rq8q2G3sSZc1Q1z5jdj4Nz8dQuieiiyDVLiKWDmtJVp Miner:Interval=10000 Blockchain:Dificulty=2 My:Host=192.168.0.127 My:Port=13000

# dotnet run Miner:Address=QXuwUXRhANuouMkHTc2tbcZhkcucShZQKEhs7XSfseSd6Rq8q2G3sSZc1Q1z5jdj4Nz8dQuieiiyDVLiKWDmtJVp Miner:Interval=10000 Blockchain:Dificulty=2 My:Host=127.0.0.1 My:Port=13001
# dotnet run Miner:Address=QXuwUXRhANuouMkHTc2tbcZhkcucShZQKEhs7XSfseSd6Rq8q2G3sSZc1Q1z5jdj4Nz8dQuieiiyDVLiKWDmtJVp Miner:Interval=10000 Blockchain:Dificulty=2 My:Host=127.0.0.1 My:Port=13002 Peer:Host=127.0.0.1 Peer:Port=13001