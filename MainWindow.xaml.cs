using System.Windows;
using System.Windows.Forms;
using Commons;
using System;
using System.Drawing;

namespace Microsoft.Kinect.Samples.CursorControl
{
    public partial class MainWindow : Window
    {
        // Sensibilidade do modulo de controle do computador
        int framesMODComputador = 5;
        float sizeGestureMODComputador = 0.03f;

        // Sensibilidade do modulo de controle do slide
        int framesMODSlide = 20;
        float sizeGestureMODSlide = 0.5f;

        // Tray Icon
        private NotifyIcon trayIcon;
        private ContextMenu trayMenu;

        // Modulos ativos
        private Boolean MODMouse = false;
        private Boolean MODSlide = false;

        // Atributo que controla se a pessoa é conhota
        private Boolean canhoto = false;

        public MainWindow()
        {
            // Criar icone da aplicação
            trayMenu = new ContextMenu();
            trayMenu.MenuItems.Add("Exibir", exibir_Click);
            trayMenu.MenuItems.Add("Sair", sair_Click); 
            trayIcon = new NotifyIcon();
            trayIcon.Text = "Kinect";
            trayIcon.Icon = new Icon(SystemIcons.Application, 40, 40);
            trayIcon.ContextMenu = trayMenu;
            trayIcon.ContextMenu = trayMenu;
            trayIcon.Visible = true;      
            // Inicializar form     
            InitializeComponent();
            // Desativar Modulos
            objModuloDesativado.IsChecked = true;
            // Definir tamanho do visualizador
            this.Left = Screen.PrimaryScreen.Bounds.Width - windowKinect.Width - 10;
            this.Top = Screen.PrimaryScreen.Bounds.Height - windowKinect.Height - 50;
        }

        // Métodos do icone - Inicio

        private void exibir_Click(object sender, EventArgs e)
        {
            this.Show();
        }

        private void sair_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // Métodos do icone - Fim

        // Métodos do menu - Inicio

        public void sair_Click(Object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        public void ocultar_Click(Object sender, RoutedEventArgs e)
        {
            this.Hide();
        }

        public void canhoto_Click(Object sender, RoutedEventArgs e)
        {
            if (objCanhoto.IsChecked)
            {
                objCanhoto.IsChecked = false;
                canhoto = false;
            }
            else
            {
                objCanhoto.IsChecked = true;
                canhoto = true;
            }
        }

        public void moduloDesativado_Click(Object sender, RoutedEventArgs e)
        {
            objModuloDesativado.IsChecked = true;
            objModuloMouse.IsChecked = false;
            MODMouse = false;
            objModuloSlide.IsChecked = false;
            MODSlide = false;
        }

        public void moduloMouse_Click(Object sender, RoutedEventArgs e)
        {
            objModuloDesativado.IsChecked = false;
            objModuloMouse.IsChecked = true;
            MODMouse = true;
            objModuloSlide.IsChecked = false;
            MODSlide = false;
        }

        public void moduloSlide_Click(Object sender, RoutedEventArgs e)
        {
            objModuloDesativado.IsChecked = false;
            objModuloMouse.IsChecked = false;
            MODMouse = false;
            objModuloSlide.IsChecked = true;
            MODSlide = true;
        }

        // Métodos do menu - Fim

        private void carregarJanela(object sender, RoutedEventArgs e)
        {
            // Ação a ser executada quando a janela é carregada
            // Preparar listener do kinect
            kinectSensor.KinectSensorChanged += new DependencyPropertyChangedEventHandler(alterarKinectSensor);
        }

        private void fecharJanela(object sender, System.EventArgs e)
        {
            // Ação a ser executada quando a janela é fechada
            // Parar kinect
            if (kinectSensor.Kinect != null)
            {
                kinectSensor.Kinect.Stop();
            }
        }

        private void alterarJanela(object sender, System.EventArgs e)
        {
            // Ação a ser executada quando a janela é alterada, minimizada
            // Ocultar janela
            if (WindowState == WindowState.Minimized)
            {
                this.Hide();
            }
        }

        private void desligarKinect(KinectSensor sensor)
        {
            // Método responsável por desligar o sensor do kinect
            if (sensor != null)
            {
                if (sensor.IsRunning)
                {
                    sensor.Stop();
                    sensor.AudioSource.Stop();
                }
            }
        }

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
            sensor.AllFramesReady += new System.EventHandler<AllFramesReadyEventArgs>(frame);
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

        void frame(object sender, AllFramesReadyEventArgs e)
        {
            // Executar o listener de movimento e imagem
            depthFrame(e);
            // Definir qual módulo será utilizado
            if (MODMouse)
                skeletonFrame(e);
            else if (MODSlide)
                skeletonFrameSlide(e);
        }

        // Atributos do modulo de controle do computador
        float posicaoInicialZMODComputador = 0f;
        float posicaoAnteriorZMODComputador = 0f;
        float posicaoAtualZMODComputador = 0f;
        int countFramesMODComputador = 0;

        void clearPosicoesClique()
        {
            posicaoInicialZMODComputador = 0;
            posicaoAnteriorZMODComputador = 0;
            posicaoAtualZMODComputador = 0;
            countFramesMODComputador = 0;
        }

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
                        if (sd.Joints[JointType.HandLeft].TrackingState == JointTrackingState.Tracked && sd.Joints[JointType.HandRight].TrackingState == JointTrackingState.Tracked)
                        {
                            // Posição do mouse
                            int mouseX, mouseY;
                            // Obter posição das mãos esquerda e direita
                            Joint maoDireita = sd.Joints[JointType.HandRight];
                            Joint maoEsquerda = sd.Joints[JointType.HandLeft];
                            // Converter de escala, a posição do kinect para a tela do computador
                            Joint maoTelaDireita = maoDireita.ScaleTo((int)SystemParameters.PrimaryScreenWidth, (int)SystemParameters.PrimaryScreenHeight, SkeletonMaxX, SkeletonMaxY);
                            Joint maoTelaEsquerda = maoEsquerda.ScaleTo((int)SystemParameters.PrimaryScreenWidth, (int)SystemParameters.PrimaryScreenHeight, SkeletonMaxX, SkeletonMaxY);
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
                                posicaoInicialZMODComputador = posicaoAtualZMODComputador;
                            }
                            else if (posicaoAtualZMODComputador < posicaoAnteriorZMODComputador)
                            {
                                // Verifica se o movimento foi maior que o anterior
                                countFramesMODComputador++;
                                if (countFramesMODComputador - 1 == framesMODComputador && posicaoInicialZMODComputador - posicaoAtualZMODComputador > sizeGestureMODComputador)
                                {
                                    // Realiza o clique do mouse
                                    NativeMethods.SendMouseInput(mouseX * 3 / 2, mouseY * 3 / 2, (int)SystemParameters.PrimaryScreenWidth, (int)SystemParameters.PrimaryScreenHeight, true);
                                    NativeMethods.SendMouseInput(mouseX * 3 / 2, mouseY * 3 / 2, (int)SystemParameters.PrimaryScreenWidth, (int)SystemParameters.PrimaryScreenHeight, false);
                                    clearPosicoesClique();
                                }
                            }
                            else
                            {
                                clearPosicoesClique();
                            }
                            posicaoAnteriorZMODComputador = posicaoAtualZMODComputador;
                            // Enviar comandos para a classe responsável pelo controle do mouse
                            NativeMethods.SendMouseInput(mouseX * 3 / 2, mouseY * 3 / 2, (int)SystemParameters.PrimaryScreenWidth, (int)SystemParameters.PrimaryScreenHeight, false);
                            return;
                        }
                    }
                }
            }
        }

        // Atributos do modulo de controle do slide
        float posicaoInicialXMODSlide = 0f;
        float posicaoAnteriorXMODSlide = 0f;
        float posicaoAtualXMODSlide = 0f;
        int countFramesMODSlide = 0;

        void clearPosicoes()
        {
            posicaoInicialXMODSlide = 0;
            posicaoAnteriorXMODSlide = 0;
            posicaoAtualXMODSlide = 0;
            countFramesMODSlide = 0;
        }

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
                        else if ((canhoto ? posicaoAtualXMODSlide < posicaoAnteriorXMODSlide : posicaoAtualXMODSlide > posicaoAnteriorXMODSlide))
                        {
                            // Verifica se o movimento foi maior que o anterior
                            countFramesMODSlide++;
                            if (countFramesMODSlide - 1 == framesMODSlide 
                                && (canhoto ? posicaoInicialXMODSlide - posicaoAtualXMODSlide > sizeGestureMODSlide : posicaoAtualXMODSlide - posicaoInicialXMODSlide > sizeGestureMODSlide))
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

        void depthFrame(AllFramesReadyEventArgs e)
        {
            // Método responsável por exibir a imagem do skeleto, caso
            // a janela esteja normal
            if (this.WindowState == WindowState.Normal)
            {
                using (DepthImageFrame depthFrame = e.OpenDepthImageFrame())
                {
                    if (depthFrame == null)
                    {
                        return;
                    }
                    kinectScreen.Source = depthFrame.ToBitmapSource();
                }
            }
        }
    }
}
