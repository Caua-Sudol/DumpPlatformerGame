# DumpPlatformerGame

Base de estudo em MonoGame para jogos de plataforma 2D. O projeto tem movimento, pulo com coyote time e jump buffer, dash em oito direcoes, colisao simples, camera, cutscene, checkpoint, pause e menu de morte.

Ele e um dump inicial: a intencao e copiar a estrutura e evoluir cada jogo sem precisar reconstruir a base de movimento.

## Controles

- `A` e `D`: mover.
- `W` e `S`: escolher direcao vertical do dash e navegar nos menus.
- `Space`: pular.
- `LeftShift`: dash.
- `Escape`: abrir ou fechar o pause.
- `Enter`: confirmar uma opcao de menu.
- `F3`: ativar ou desativar o debug visual.

## Estrutura

| Pasta | Responsabilidade |
| --- | --- |
| `App` | Loop principal, tela ativa e overlays. |
| `Core` | Camera e debug visual reutilizaveis. |
| `Gameplay` | Player, leitura de input e colisao do player. |
| `Scenes` | Fluxo da fase: jogo, fade, cutscene, respawn e camera. |
| `UI` | Menus de inicio, pause e morte. |
| `World` | Dados do TMX, plataformas e triggers. |
| `docs` | Metricas de movimento e guias do template. |

`Game1` decide qual tela esta ativa. `Scene` coordena a fase, mas nao interpreta o TMX nem resolve a colisao diretamente. `LevelMap` le os objetos do mapa e `PlayerCollision` aplica a colisao contra as plataformas.

## Criar uma fase

O construtor de `Scene` aceita o caminho do TMX e o spawn inicial:

```csharp
new Scene("Content/MinhaFase.tmx", new Vector2(32, 544));
```

O TMX precisa manter estas object layers:

- `mapaGeral`: retangulos solidos usados pela colisao e pelo desenho de teste.
- `triggerColision`: retangulos que iniciam a cutscene de exemplo.

As medidas atuais de movimento e uma regua para desenhar no Tiled estao em [docs/metricas_mov.md](docs/metricas_mov.md).

## Debug visual

Com `F3` ativo, o jogo mostra dados do mundo sem abrir console ou imprimir varios textos:

- contorno verde das plataformas;
- trigger da cutscene em vermelho;
- checkpoint em magenta e alvo da camera em ciano;
- hitbox do player em verde no chao, amarelo no ar e laranja durante o dash.

## Pontos especificos do exemplo

O mapa padrao, spawn e valores da cutscene atual existem apenas para demonstrar o fluxo. Ao criar outro jogo, ajuste primeiro o caminho do TMX e o spawn; depois revise os valores da cutscene em `Scene` conforme a fase nova.

## Limites atuais

- O jogo usa `1920x1080` como resolucao logica e escala esse quadro para a janela atual, inclusive ao redimensionar a janela.
- Telas com outra proporcao podem mostrar barras pretas para preservar as medidas do jogo.
- A cutscene atual e unica e usa uma trigger simples.
- Nao existe sistema de vida ou dano, apenas menu de morte por queda.

## Executar

```bash
dotnet restore
dotnet run
```
