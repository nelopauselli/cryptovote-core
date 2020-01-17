FROM mcr.microsoft.com/dotnet/core/aspnet:3.1

WORKDIR /app

ADD ./crypto-vote-linux-x64.tar.gz /app

RUN chmod +x /app/crypto-vote

EXPOSE 80

ENTRYPOINT ["/app/crypto-vote"]