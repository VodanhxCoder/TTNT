using A_BFS;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;



namespace WindowsFormsApplication1
{
    public partial class frmPuzzleGame : Form
    {
        int chiSoOTrong, soBuocDi = 0;
        List<Bitmap> mangGoc = new List<Bitmap>();
		//khởi tạo phương thức đo thời gian trôi qua
        System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();

        List<State> ketQuaCuoiCung = new List<State>();
        int currentState = 0;

		List<int> mangCuoi = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

		// Test case 
		List<int> tesCase1 = new List<int> { 1, 2, 9, 3, 4, 6, 7, 5, 8 }; //trường hợp đb bfs 15/ tối ưu 89
		List<int> tesCase2 = new List<int> { 4, 5, 9, 3, 1, 6, 7, 2, 8 }; //15 nhưng khác số bước duyệt
		List<int> tesCase3 = new List<int> { 4, 9, 5, 3, 1, 6, 7, 2, 8 }; //14 tương tự như 15
		List<int> tesCase4 = new List<int> { 9, 1, 2, 3, 6, 5, 4, 8, 7 }; //bfs không ra, tối ưu ra 53
		List<int> tesCase5 = new List<int> { 9, 1, 3, 2, 6, 5, 4, 7, 8 }; //bfs 15 nhưng lâu, tối ưu 37 nhưng nhanh


		List<List<int>> mangTestCase = new List<List<int>>();

		public frmPuzzleGame()
        {
            InitializeComponent();
			//khởi tạo mảng gốc để so sánh với kqua người chơi
			mangGoc.AddRange(new Bitmap[] { Properties.Resources._1, Properties.Resources._2, Properties.Resources._3, Properties.Resources._4, 
			Properties.Resources._5, Properties.Resources._6, Properties.Resources._7, Properties.Resources._8, Properties.Resources._null });
            gbAnhGoc.BackgroundImage = Properties.Resources.original;
            lblBuocDi.Text += soBuocDi;
            lblThoiGianDem.Text = "00:00:00";

			// Add test case
			mangTestCase.Add(tesCase1);
			mangTestCase.Add(tesCase2);
			mangTestCase.Add(tesCase3);
			mangTestCase.Add(tesCase4);
			mangTestCase.Add(tesCase5);
		}

        private void Form1_Load(object sender, EventArgs e)
        {
            ChoiLai();
        }
        private int TinhSoDaoNguoc(List<int> mang)
        {
            int soDaoNguoc = 0;
            for (int i = 0; i < mang.Count; i++)
            {
                if (mang[i] == 9) continue;
                for (int j = i + 1; j < mang.Count; j++)
                {
                    if (mang[j] == 9) continue;
                    if (mang[i] > mang[j])
                    {
                        soDaoNguoc++;
                    }
                }
            }
            return soDaoNguoc;
        }
        List<int> ChoiLai()
        {
            List<int> mangRandom = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            Random rand = new Random();
            State trangThaiDich = new State(mangCuoi);
            int soDaoNguocDich = TinhSoDaoNguoc(mangCuoi); // Tính một lần
            int maxAttempts = 20; 

            for (int attempt = 0; attempt < maxAttempts; attempt++)
            {
                // Xáo trộn mảng
                for (int i = mangRandom.Count - 1; i > 0; i--)
                {
                    int j = rand.Next(0, i + 1);
                    int temp = mangRandom[i];
                    mangRandom[i] = mangRandom[j];
                    mangRandom[j] = temp;
                }

                // Kiểm tra tính khả thi
                State trangThaiDau = new State(mangRandom);
                if (!trangThaiDau.KiemTraBatKhaThi(trangThaiDich, soDaoNguocDich))
                {
                    // Cập nhật giao diện
                    try
                    {
                        for (int i = 0; i < 9; i++)
                        {
                            if (mangGoc.Count > mangRandom[i] - 1 && mangGoc[mangRandom[i] - 1] != null)
                            {
                                ((PictureBox)gbKhung.Controls[i]).Image = mangGoc[mangRandom[i] - 1];
                                if (mangRandom[i] == 9)
                                    chiSoOTrong = i;
                            }
                            else
                            {
                                throw new Exception("Hình ảnh trong mangGoc không hợp lệ.");
                            }
                        }
                        // Kiểm tra trạng thái thắng
                        if (!KiemTraWin())
                        {
                            Application.DoEvents();
                            return mangRandom;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Lỗi cập nhật giao diện: {ex.Message}");
                        continue;
                    }
                }
            }

            // Dùng test case mặc định
            mangRandom = mangTestCase[0];
            try
            {
                for (int i = 0; i < 9; i++)
                {
                    ((PictureBox)gbKhung.Controls[i]).Image = mangGoc[mangRandom[i] - 1];
                    if (mangRandom[i] == 9)
                        chiSoOTrong = i;
                }
                Application.DoEvents();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi dùng test case mặc định: {ex.Message}");
            }
            return mangRandom;

            //Random r = new Random();
            //int j = r.Next(0, 5);
            ////j = 2;
            //List<int> mangRandom = mangTestCase[j];

            //do
            //{
            //    for (int i = 0; i < 9; i++)
            //    {
            //        ((PictureBox)gbKhung.Controls[i]).Image = mangGoc[mangRandom[i] - 1];
            //        if (mangRandom[i] == 9)
            //            chiSoOTrong = i;
            //    }
            //} while (KiemTraWin());
            //return mangRandom;
        }

        private void btnChoiLai_Click(object sender, EventArgs e)
        {
            DialogResult YesOrNo = new DialogResult();
            if (lblThoiGianDem.Text != "00:00:00")
            {
                YesOrNo = MessageBox.Show("Bạn có muốn chơi lại hay không?", "Game Ghép Hình", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            }
            if (YesOrNo == DialogResult.Yes || lblThoiGianDem.Text == "00:00:00")
            {
                // Đặt lại trạng thái chương trình
                ketQuaCuoiCung.Clear(); // Xóa đường đi cũ
                currentState = 0; // Đặt lại chỉ số trạng thái
                soBuocDi = 0;
                lblBuocDi.Text = "Số Bước Đi: 0";
                lblBuocDuyet.Text = "Số bước duyệt: 0";
                lblTimeGiai.Text = "Thời gian giải: 0.0 ms";
                timer.Stop();
                timer.Reset();
                lblThoiGianDem.Text = "00:00:00";

              

                // Gọi ChoiLai để tạo trạng thái mới
                try
                {
                    ChoiLai();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi tạo trạng thái mới: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void KiemTraThoatChuongTrinh(object sender, FormClosingEventArgs e)
        {
            DialogResult YesOrNO = MessageBox.Show("Bạn có muốn thoát chương trình hay không ?", "Game Ghép Hình", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (sender as Button != btnThoat && YesOrNO == DialogResult.No) e.Cancel = true;
            if (sender as Button == btnThoat && YesOrNO == DialogResult.Yes) Environment.Exit(0);
        }

		private void btnThoat_Click(object sender, EventArgs e)
		{
			KiemTraThoatChuongTrinh(sender, e as FormClosingEventArgs);
		}

        private void btnGiai_Click(object sender, EventArgs e)
        {
           
			
		}


		private void CachThucDiChuyen(object sender, EventArgs e)
        {
            if (lblThoiGianDem.Text == "00:00:00")
                timer.Start();
            int oNguoiDungChon = gbKhung.Controls.IndexOf(sender as Control);
            if (chiSoOTrong != oNguoiDungChon)
            {
				//tìm ra danh sách những số họ hàng với số ô người dùng chọn
                List<int> danhSachCacChiSoHoHang = new List<int>(new int[] { ((oNguoiDungChon % 3 == 0) ? -1 : oNguoiDungChon - 1), oNguoiDungChon - 3, (oNguoiDungChon % 3 == 2) ? -1 : oNguoiDungChon + 1, oNguoiDungChon + 3 });
				//nếu ô đen trống mà nằm trong ds này có nghĩa là có thể đi được.
                if (danhSachCacChiSoHoHang.Contains(chiSoOTrong))
                {
					//gán ô trống thành ô hình người dùng chọn, và gán ô người dùng chọn thành ô đen trống là ptu thứ 9 trong mảng gốc
                    ((PictureBox)gbKhung.Controls[chiSoOTrong]).Image = ((PictureBox)gbKhung.Controls[oNguoiDungChon]).Image;
                    ((PictureBox)gbKhung.Controls[oNguoiDungChon]).Image = mangGoc[8];
					chiSoOTrong = oNguoiDungChon;
                    lblBuocDi.Text = "Số Bước Đi: " + (++soBuocDi);
                    if (KiemTraWin())
                    {
                        timer.Stop();
                        (gbKhung.Controls[8] as PictureBox).Image = mangGoc[8];
                        MessageBox.Show("Chúc mừng bạn đã chiến thắng game...\nThời gian là : " + timer.Elapsed.ToString().Remove(8) + "\nSố Bước Đi : " + soBuocDi, "Game Ghép Hình");
						soBuocDi = 0;
                        lblBuocDi.Text = "Số Bước Đi: 0";
                        lblThoiGianDem.Text = "00:00:00";
                        timer.Reset();
						ChoiLai();
                    }
                }
            }
        }

        bool KiemTraWin()
        {
			//so với mảng gốc là hình ảnh gốc đã set theo thứ tự ban đầu, nếu trong quá trình duyệt có cái nào k đúng
			//thì lập tức break ra =>false, ngược lại đến 8 vẫn đúng thì trả về true
            int i;
            for (i = 0; i < 8; i++)
            {
                if ((gbKhung.Controls[i] as PictureBox).Image != mangGoc[i])
					break;
            }
            if (i == 8) return true;
            else return false;
        }

        private void TinhThoiGian(object sender, EventArgs e)
        {
			//nếu người chơi bắt đầu click vào các ô ngoại trừ ô số 8 là ô đen trống thì bắt đầu đếm thời gian
            if (timer.Elapsed.ToString() != "00:00:00")
                lblThoiGianDem.Text = timer.Elapsed.ToString().Remove(8);
            if (timer.Elapsed.ToString() == "00:00:00")
                btnTamDung.Enabled = false;
            else
                btnTamDung.Enabled = true;
        }

        private void btnDiLui_Click(object sender, EventArgs e)
        {
			//nút lui lại đến trạng thái cuối cùng rồi thì ngưng nên mới có điều kiện > 0
			if(currentState > 0)
			{
				currentState -= 1;

				this.lblBuocDi.Text = "Số Bước Đi: " + (currentState + 1).ToString() + "/" + this.ketQuaCuoiCung.Count.ToString();

				State trt = ketQuaCuoiCung[currentState];

				for (int j = 0; j < trt.trangThai.Count; j++)
				{
					((PictureBox)gbKhung.Controls[j]).Image = mangGoc[trt.trangThai[j] - 1];
				}
			}		
		}

        private void btnDiToi_Click(object sender, EventArgs e)
        {
			//nút lui tới đến trạng thái cuối cùng rồi thì ngưng nên mới có điều kiện < chiều dài của dsach đường đi tìm dc
			if (currentState < ketQuaCuoiCung.Count - 1)
			{
				currentState += 1;

				this.lblBuocDi.Text = "Số Bước Đi: " + (currentState + 1).ToString() + "/" + this.ketQuaCuoiCung.Count.ToString();

				State trt = ketQuaCuoiCung[currentState];

				for (int j = 0; j < trt.trangThai.Count; j++)
				{
					((PictureBox)gbKhung.Controls[j]).Image = mangGoc[trt.trangThai[j] - 1];
				}
			}				
		}

		private void btnGiaiToiUu_Click(object sender, EventArgs e)
		{
            btnGiaiToiUu.Text = "Đang giải ...";
            btnGiaiToiUu.Enabled = false;
            

            List<int> mangDau = ChoiLai();
			
			State trThaiDau = new State(mangDau);
			State trThaiCuoi = new State(mangCuoi);
			BFS bfs = new BFS(trThaiDau, trThaiCuoi);

			Stopwatch timer_tmp = new Stopwatch();
			timer_tmp.Reset();
			lblTimeGiai.Text = "Thời gian giải: 0.0 ms";
			timer_tmp.Start();

			this.ketQuaCuoiCung = bfs.Solve_BestFirstSearch();
			timer.Stop();
			lblTimeGiai.Text = "Thời gian giải: " + timer_tmp.Elapsed.TotalMilliseconds.ToString() + " ms";

			//đảo mảng lại vì kqua tra về là đường đi ngược từ dưới lên
			this.ketQuaCuoiCung.Reverse();
			this.lblBuocDuyet.Text = "Số Bước Duyệt: " + bfs.dem.ToString();

			this.currentState = 0;
			this.lblBuocDi.Text = "Số Bước Đi: " + (currentState + 1).ToString() + "/" + this.ketQuaCuoiCung.Count.ToString();
			State tmp = this.ketQuaCuoiCung[this.currentState];
			List<int> mang = tmp.trangThai;
			for (int j = 0; j < mang.Count; j++)
			{
				((PictureBox)gbKhung.Controls[j]).Image = mangGoc[mang[j] - 1];
			}
            btnGiaiToiUu.Text = "Best Frist Search";
            btnGiaiToiUu.Enabled = true;
        }

		private void lblTimeGiai_Click(object sender, EventArgs e)
		{

		}

        private void lblThoiGianDem_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
          
        }

        private void gbAnhGoc_Enter(object sender, EventArgs e)
        {

        }

        private void button1_Click_1(object sender, EventArgs e)
        {           
            using (OpenFileDialog openFile = new OpenFileDialog())
            {
               
                openFile.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";

                if (openFile.ShowDialog() == DialogResult.OK)
                {
                    string filePath = openFile.FileName;


                    string saveFolder = @"C:\images\tiles";

                    // Xử lý ảnh: crop vùng giữa và cắt 9 ô
                    var result = ImageProcessor.ProcessImageWithResized(filePath, 1731, 1047);
                    Bitmap[] tiles = result.tiles;
                    Bitmap resizedImage = result.resizedImage;
                  
                    mangGoc.Clear();
                    // Thêm các ô đã cắt vào mangGoc
                    // Thêm 8 ô đầu tiên (index 0 đến 7)
                    
                    for (int i = 0; i < 8; i++)
                    {
                        mangGoc.Add(tiles[i]);

                    }
                    mangGoc.Add(Properties.Resources._null);
                    gbAnhGoc.BackgroundImage = resizedImage;



                    MessageBox.Show("Tải ảnh và cắt thành 9 ô thành công!");
                    btnChoiLai_Click(sender, e);



                }
            }
        }



        //sự kiện click và nút tạm dừng, tạm dừng thì tắt màn hình lại không cho người chơi xem các ô nữa
        private void PauseOrResume(object sender, EventArgs e)
        {
            if (btnTamDung.Text == "Tạm Dừng")
            {
                timer.Stop();
                gbKhung.Visible = false;
                btnTamDung.Text = "Tiếp Tục";
            }
            else
            {
                timer.Start();
                gbKhung.Visible = true;
                btnTamDung.Text = "Tạm Dừng";
            }
        }
    }
}
