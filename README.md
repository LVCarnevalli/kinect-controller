# kinect-controller
Projeto de TCC, têm como objetivo controlar o mouse do computador pelo Kinect.

# Como funciona?
Neste trabalho, foi desenvolvido um aplicativo computacional utilizando o sensor de movimentos Kinect. 

A ideia principal é auxiliar e, também, facilitar, a interação de pessoas portadoras de alguma deficiência com o computador.
Para alcançar este objetivo, foi utilizada a linguagem de programação C# e a tecnologia de camada de visualização escolhida para o desenvolvimento foi o WTF, que permite desenho de objetos 2D e 3D. 

O SDK foi utilizado como API de comunicação com o Kinect. Este SDK inclui drivers para o uso do sensor de movimentos da Microsoft. O software Developer Toolkit também foi usado no desenvolvimento, oferecendo diversos recursos, entre eles um pacote com diretrizes de interface humana, aplicativos de amostrar e API’s de controle de faces mais complexas.

Em todas as tarefas descritas foram utilizadas o Visual Studio Premium 2013.

![](https://github.com/LVCarnevalli/kinect-controller/blob/master/resources/image1.png)

> Tela principal do aplicativo, quando o Kinect não está conectado no computador, é exibido uma mensagem padrão.

O protótipo foi divido em dois módulos: o de controle do mouse e o controle de apresentação, onde cada um tem uma função diferente a ser realizada pelo sistema.

![](https://github.com/LVCarnevalli/kinect-controller/blob/master/resources/image2.png)

> Tela do aplicativo exibindo os módulos que estão ativos.

## Módulo de controle do mouse ##
O módulo de controle do mouse é necessário para que o usuário consiga, através das mãos, controlar o mouse no computador. Com essa função, é possível realizar movimentos e clique com o botão direito do mouse.

Esse módulo possui dois movimentos:

- 1° Movimento

Ação: Mover a mão para esquerda, direita, cima e baixo, em direção ao Kinect.

Resultado: O aplicativo move o ponteiro do mouse em relação a posição da mão. 

- 2° Movimento

Ação: Mover a mão para frente e voltar à posição inicial, em direção ao Kinect.

Resultado: O aplicativo identifica que foi realizado um movimento de profundidade e realiza o clique com o botão direito do mouse.

## Módulo de controle de apresentação ##
O módulo de controle de apresentação é necessário para que o usuário consiga, através das mãos, mudar de slide em uma apresentação. Para isso, é necessário apenas realizar um movimento.

- 1º Movimento

Ação: Mover a mão direita para esquerda e retornar para a posição inicial, em direção ao Kinect. Caso o usuário seja canhoto é necessário mover a mão esquerda para a direita e retornar a posição inicial, em direção ao Kinect.

Resultado: O aplicativo identifica o movimento e muda o slide da apresentação.

## Outras funções ##

Para as pessoas canhotas, foi criada a função do Canhoto nos dois módulos, onde é possível definir que o usuário é canhoto. Sendo assim, a direção dos movimentos é alterada para o lado inverso.

![](https://github.com/LVCarnevalli/kinect-controller/blob/master/resources/image3.png)

> Tela do aplicativo com a função canhoto.

O aplicativo também implementa a função de Ocultar, caso seja necessário ocultar a tela do aplicativo.

![](https://github.com/LVCarnevalli/kinect-controller/blob/master/resources/image4.png)

![](https://github.com/LVCarnevalli/kinect-controller/blob/master/resources/image5.png)

> Tela do aplicativo com a função oculto.

Após um dos módulos ser ativos e o Kinect estando conectado no computador, o aplicativo exibe o esqueleto da pessoa na tela do aplicativo.

![](https://github.com/LVCarnevalli/kinect-controller/blob/master/resources/image6.png)

> Tela do aplicativo funcionando.

## Plano de Desenvolvimento ##

O desenvolvimento se deu por início na escolha do SDK a ser utilizado, existem várias bibliotecas de conexão e manipulação do Kinect, porém as bibliotecas originais da SDK Microsoft são as que menos dão erros e as que estão mais bens documentadas, esses foram os principais motivos da escolha do mesmo.

Após a escolha do SDK era necessário escolher um Viewer que é responsável por capturar as imagens do Kinect e exibir na tela do computador, o qual utilizamos foi o “Kinect Wpf Viewers” que é próprio da Microsoft, que pode ser encontrado no pacote Developer Toolkit, exemplos de aplicação Kinect.

Passando por essas etapas já foi possível iniciar o desenvolvimento da aplicação que foi dividida nas seguintes partes:

- ImageCommon
- 
Esse módulo é responsável pelo controle e a manipulação gráfica, entre o Kinect e a tela do computador.

- Mouse Common
- 
Esse módulo é responsável pela manipulação do mouse, é possível executar qualquer ação no mouse.

- Skeletal Common
- 
Esse módulo é responsável por exibir o esqueleto do usuário na tela do computador.

- Main Window
- 
Esse módulo é o principal do aplicativo, é nele que consiste a implementação de todos os módulos e toda a personalização do aplicativo.

O núcleo do aplicativo está nas técnicas implementadas dos módulos.
A função responsável pelo módulo de controle do mouse é a skeletonFrame:


    void skeletonFrame(AllFramesReadyEventArgs e)
        {
            float SkeletonMaxX = 0.60f;
            float SkeletonMaxY = 0.40f;
            // Método responsável por capturar todos os movimentos realizados
            // pelo skeleto
            using (SkeletonFrame data = e.OpenSkeletonFrame())
            {
                if (data == null)
                {
                    return;
                }
                // Recuperar os movimentos
                int countImagens = data.SkeletonArrayLength;
                Skeleton[] allSkeletons = new Skeleton[countImagens];
                data.CopySkeletonDataTo(allSkeletons);
                // Verificar os movimentos
                foreach (Skeleton sd in allSkeletons)
                {
                    // Verificar se todos os movimentos foram capturados
                    if (sd.TrackingState == SkeletonTrackingState.Tracked)
                    {
                        // Verifica se as duas mãos foram capturadas
                        if (sd.Joints[JointType.HandLeft].TrackingState == JointTrack-ingState.Tracked && sd.Joints[JointType.HandRight].TrackingState == JointTrack-ingState.Tracked)
                        {
                            // Posição do mouse
                            int mouseX, mouseY;
                            // Obter posição das mãos esquerda e direita
                            Joint maoDireita = sd.Joints[JointType.HandRight];
                            Joint maoEsquerda = sd.Joints[JointType.HandLeft];
                            // Converter de escala, a posição do kinect para a tela do computador
                            Joint maoTelaDireita = maoDireita.ScaleTo((int)SystemParameters.PrimaryScreenWidth, (int)SystemParameters.PrimaryScreenHeight, SkeletonMaxX, SkeletonMaxY);
                            Joint maoTelaEsquerda = maoEsquer-da.ScaleTo((int)SystemParameters.PrimaryScreenWidth, (int)SystemParameters.PrimaryScreenHeight, SkeletonMaxX, SkeletonMaxY);
                            // Definir posição do mouse
                            // Verificar se a mão esquerda está sendo utilizada
                            // para mover o mouse
                            if (canhoto)
                            {
                                // Definir posição do cursor do mouse
                                // de acordo com a mão esquerda
                                mouseX = (int)maoTelaEsquerda.Position.X;
                                mouseY = (int)maoTelaEsquerda.Position.Y;
                            }
                            else
                            {
                                // Definir posição do cursor do mouse
                                // de acordo com a mão direita
                                mouseX = (int)maoTelaDireita.Position.X;
                                mouseY = (int)maoTelaDireita.Position.Y;
                            }

                            // Recuperar posição atual da mão (profundidade)
                            if (canhoto)
                            {
                                posicaoAtualZMODComputador = sd.Joints[JointType.HandLeft].Position.Z;
                            }
                            else
                            {
                                posicaoAtualZMODComputador = sd.Joints[JointType.HandRight].Position.Z;
                            }
                            
                            if (posicaoInicialZMODComputador == 0)
                            {
                                // Se o movimento for iniciado, define valor
                                posicaoInicialZMODComputador = posicaoAtualZMODCompu-tador;
                            }
                            else if (posicaoAtualZMODComputador < posicaoAnteriorZMO-DComputador)
                            {
                                // Verifica se o movimento foi maior que o anterior
                                countFramesMODComputador++;
                                if (countFramesMODComputador - 1 == framesMODComputa-dor && posicaoInicialZMODComputador - posicaoAtualZMODComputador > sizeGestureMODCom-putador)
                                {
                                    // Realiza o clique do mouse
                                    NativeMethods.SendMouseInput(mouseX * 3 / 2, mous-eY * 3 / 2, (int)SystemParameters.PrimaryScreenWidth, (int)SystemParameters.PrimaryScreenHeight, true);
                                    NativeMethods.SendMouseInput(mouseX * 3 / 2, mous-eY * 3 / 2, (int)SystemParameters.PrimaryScreenWidth, (int)SystemParameters.PrimaryScreenHeight, false);
                                    clearPosicoesClique();
                                }
                            }
                            else
                            {
                                clearPosicoesClique();
                            }
                            posicaoAnteriorZMODComputador = posicaoAtualZMODComputa-dor;
                            // Enviar comandos para a classe responsável pelo controle do mouse
                            NativeMethods.SendMouseInput(mouseX * 3 / 2, mouseY * 3 / 2, (int)SystemParameters.PrimaryScreenWidth, (int)SystemParameters.PrimaryScreenHeight, false);
                            return;
                        }
                    }
                }
            }
        }

A função responsável pelo módulo de controle da apresentação é a skeletonFrameSlide:

    void skeletonFrameSlide(AllFramesReadyEventArgs e)
        {
            using (SkeletonFrame data = e.OpenSkeletonFrame())
            {
                if (data == null)
                {
                    return;
                }
                // Recuperar os movimentos
                int countImagens = data.SkeletonArrayLength;
                Skeleton[] allSkeletons = new Skeleton[countImagens];
                data.CopySkeletonDataTo(allSkeletons);
                // Verificar os movimentos
                foreach (Skeleton sd in allSkeletons)
                {
                    // Verificar se todos os movimentos foram capturados
                    if (sd.TrackingState == SkeletonTrackingState.Tracked)
                    {
                        // Recuperar posição atual da mão (movimento horinzontal)
                        posicaoAtualXMODSlide = (canhoto ? sd.Joints[JointType.HandLeft].Position.X : sd.Joints[JointType.HandRight].Position.X);
                 
                        if (posicaoInicialXMODSlide == 0)
                        {
                            // Se o movimento for iniciado, define valor
                            posicaoInicialXMODSlide = posicaoAtualXMODSlide;
                        }
                        else if ((canhoto ? posicaoAtualXMODSlide < posicaoAnteriorX-MODSlide : posicaoAtualXMODSlide > posicaoAnteriorXMODSlide))
                        {
                            // Verifica se o movimento foi maior que o anterior
                            countFramesMODSlide++;
                            if (countFramesMODSlide - 1 == framesMODSlide 
                                && (canhoto ? posicaoInicialXMODSlide - posicaoAtualX-MODSlide > sizeGestureMODSlide : posicaoAtualXMODSlide - posicaoInicialXMODSlide > sizeGestureMODSlide))
                            {
                                // Muda de slide
                                NativeMethods.SendMouseInput(50, 50, (int)SystemParameters.PrimaryScreenWidth, (int)SystemParameters.PrimaryScreenHeight, true);
                                NativeMethods.SendMouseInput(50, 50, (int)SystemParameters.PrimaryScreenWidth, (int)SystemParameters.PrimaryScreenHeight, false);
                                clearPosicoes();
                            }
                        }
                        else
                        {
                            clearPosicoes();
                        }
                        // Definir posição anterior pela atual, pois ação já foi feita
                        posicaoAnteriorXMODSlide = posicaoAtualXMODSlide;
                    }
                }
            }
        }

A técnica de captura de movimentos implementada nos dois módulos segue o padrão do sistema armazenar a posição inicial da mão e após alguns milésimos de segundos e uma quantidade de frame parametrizada o sistema obtém a posição final da mão. Sendo assim, um cálculo será feito, que é a posição final menos a posição inicial e se o resultado superar o valor parametrizado a ação será realizada.

Como vide exemplo abaixo:

1. Posição inicial da mão: 12X e 3Y
1. Posição final da mão 24X e 4Y
1. O sistema identificou que da posição inicial para final levou 10 frames.
1. A diferença da coordenada horizontal (X) foi de 12.

Se o sistema estiver parametrizado para uma quantidade de frames inferior a 10 e uma diferença de coordenada inferior a 12, a ação será realizada, caso contrário nenhuma ação será realizada e o sistema continua procurando movimentos válidos de acordo com os valores parametrizados.

O módulo de controle de mouse utiliza a coordenada de profundidade para realizar o clique, já o módulo de controle de apresentação utiliza a coordenada horizontal, porém o cálculo segue a mesma lógica.

Para o funcionamento correto de todos os módulos é necessário que o Kinect esteja identificando e processando todos os frames, para isso existe uma função thread que é executada na aplicação e de tempos e tempos fica capturando tudo que acontece no Kinect, para cada informação capturada a ação configurada pelo programador é executada. Sendo assim é possível determinar uma ação para cada padrão de movimento obtido pelo Kinect.

Nessa mesma função é possível parametrizar todas as configurações de como o Kinect vai capturar e processar as imagens capturadas, resolução, cores entre outros. A função pode ser demonstrada da seguinte forma:

    void alterarKinectSensor(object sender, DependencyPropertyChangedEventArgs e)
        {
            KinectSensor old = (KinectSensor)e.OldValue;
            // Desligar o kinect com os valores antigos,
            // e iniciar com os valor novos
            desligarKinect(old);
            KinectSensor sensor = (KinectSensor)e.NewValue;
            if (sensor == null)
            {
                return;
            }
            // Parâmetros de configuração do skeleto e do sensor
            TransformSmoothParameters parameters = new TransformSmoothParameters();
            parameters.Smoothing = 0.7f;
            parameters.Correction = 0.3f;
            parameters.Prediction = 0.4f;
            parameters.JitterRadius = 1.0f;
            parameters.MaxDeviationRadius = 0.5f;
            sensor.SkeletonStream.Enable(parameters);
            sensor.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);
            sensor.DepthStream.Enable(DepthImageFormat.Resolution320x240Fps30);
            sensor.AllFramesReady += new Sys-tem.EventHandler<AllFramesReadyEventArgs>(frame);
            try
            {
                // Iniciar sensor do kinect
                sensor.Start();
            }
            catch (System.IO.IOException)
            {
                // Lança exceção que o kinect está sendo utilizado
                kinectSensor.AppConflictOccurred();
            }
        }


