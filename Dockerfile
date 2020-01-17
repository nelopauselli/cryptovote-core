FROM mcr.microsoft.com/dotnet/core/aspnet:3.1

WORKDIR /app

ADD ./cryptovote-linux-x64.tar.gz /app

RUN chmod +x /app/CryptoVote

EXPOSE 80

ENTRYPOINT ["/app/CryptoVote"]