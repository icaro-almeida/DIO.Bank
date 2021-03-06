# Digital Innovation One - Prática .NET

## POO na prática: criando uma aplicação bancária

Desenvolvido como atividade do Bootcamp "LocalizaLabs .NET Developer" na Digital Innovation One.

### Informações importantes ao usuário: 
Usuário padrão: admin Senha: admin
Para sair a qualquer momento digite "sair", "fechar","exit", "quit", ou feche a janela. Os dados são salvos normalmente a cada modificação e ao encerrar.

## Detalhes:
Projeto de sistema para operador bancário com funcionalidades de saque, transferência, depósito, listar, adicionar, excluir e trocar senha de contas de cliente e operador.
Implementa funcionalidade de login e logoff de operador.

- Uso de herança, onde as classes Cliente e Operador herdam da classe Usuario
- Persistência dos cadastros através da classe ArmazenaDados com BinnaryFormatter.Serialize (Obsoleto)
- Class Password para armazenamento e verificação de senhas usando hash e salt
- Logging através da plataforma NLog
- Classe EeS de validação de entradas do usuário, como entradas integer e double 
- Uso de event handler no encerramento do programa para salvar os dados de usuários
- Uso de Try-Catch com log nas exceções

## Contato

Ícaro Rocha de Almeida

[Linkedin](https://www.linkedin.com/in/ícaro-rocha-de-almeida/)

[YouTube](https://www.youtube.com/channel/UCTpkO-L3pK4nCA52ro8T0BA)

[Github](https://github.com/icaro-almeida)
