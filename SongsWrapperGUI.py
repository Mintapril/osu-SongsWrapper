import sys
from PyQt5 import QtCore, QtWidgets
import osual

print("Osu SongsWrapper beta  \nCreate by Mint")
######GUI部件......
class Ui_Form(object):
    def setupUi(self, Form):
        Form.setObjectName("Form")
        Form.setEnabled(True)
        Form.resize(525, 257)
        Form.setMinimumSize(QtCore.QSize(525, 257))
        Form.setMaximumSize(QtCore.QSize(525, 257))
        self.Start = QtWidgets.QPushButton(Form)
        self.Start.setGeometry(QtCore.QRect(270, 166, 160, 61))
        self.Start.setObjectName("Start")
        self.splitter_2 = QtWidgets.QSplitter(Form)
        self.splitter_2.setGeometry(QtCore.QRect(140, 30, 321, 111))
        self.splitter_2.setOrientation(QtCore.Qt.Vertical)
        self.splitter_2.setObjectName("splitter_2")
        self.Packname = QtWidgets.QLineEdit(self.splitter_2)
        self.Packname.setEnabled(True)
        self.Packname.setObjectName("Packname")
        self.Artist = QtWidgets.QLineEdit(self.splitter_2)
        self.Artist.setEnabled(True)
        self.Artist.setObjectName("Artist")
        self.Creator = QtWidgets.QLineEdit(self.splitter_2)
        self.Creator.setEnabled(True)
        self.Creator.setObjectName("Creator")
        self.splitter = QtWidgets.QSplitter(Form)
        self.splitter.setGeometry(QtCore.QRect(53, 30, 110, 111))
        self.splitter.setOrientation(QtCore.Qt.Vertical)
        self.splitter.setObjectName("splitter")
        self.label1 = QtWidgets.QLabel(self.splitter)
        self.label1.setObjectName("label1")
        self.label = QtWidgets.QLabel(self.splitter)
        self.label.setObjectName("label")
        self.label_3 = QtWidgets.QLabel(self.splitter)
        self.label_3.setObjectName("label_3")
        self.label_5 = QtWidgets.QLabel(Form)
        self.label_5.setGeometry(QtCore.QRect(80, 164, 41, 25))
        self.label_5.setObjectName("label_5")
        self.label_6 = QtWidgets.QLabel(Form)
        self.label_6.setGeometry(QtCore.QRect(80, 204, 41, 25))
        self.label_6.setObjectName("label_6")
        self.splitter_3 = QtWidgets.QSplitter(Form)
        self.splitter_3.setGeometry(QtCore.QRect(140, 160, 91, 71))
        self.splitter_3.setOrientation(QtCore.Qt.Vertical)
        self.splitter_3.setObjectName("splitter_3")
        self.OD = QtWidgets.QLineEdit(self.splitter_3)
        self.OD.setEnabled(True)
        self.OD.setMaximumSize(QtCore.QSize(125, 16777215))
        self.OD.setObjectName("OD")
        self.HP = QtWidgets.QLineEdit(self.splitter_3)
        self.HP.setEnabled(True)
        self.HP.setObjectName("HP")
        self.splitter_3.raise_()
        self.Start.raise_()
        self.splitter_2.raise_()
        self.label_5.raise_()
        self.label_6.raise_()
        self.splitter.raise_()
        self.retranslateUi(Form)
        self.Start.clicked.connect(self.getext)
        QtCore.QMetaObject.connectSlotsByName(Form)

    def getext(self):      ####实现点击按钮读取5个文本框内容
        pn = self.Packname.text()
        ar = self.Artist.text()
        cr = self.Creator.text()
        odr = self.OD.text()
        hpr = self.HP.text()
        artist, packname, path = osual.startwrapper(pn, ar, cr, odr, hpr)    ####开始处理...... 
        path = osual.zipfile(artist, packname, path)
        osual.removef(path)
        print("Packaged successfully!")

    def retranslateUi(self, Form):
        _translate = QtCore.QCoreApplication.translate
        Form.setWindowTitle(_translate("Form", "Songs Wrapper"))
        self.Start.setText(_translate("Form", "Start"))
        self.Packname.setText(_translate("Form", "Collection 1st"))
        self.Artist.setText(_translate("Form", "Various"))
        self.label1.setText(_translate("Form", "Pack Name:"))
        self.label.setText(_translate("Form", "Artist:"))
        self.label_3.setText(_translate("Form", "Creator:"))
        self.label_5.setText(_translate("Form", "OD:"))
        self.label_6.setText(_translate("Form", "HP:"))
        self.OD.setText(_translate("Form", "9"))
        self.HP.setText(_translate("Form", "7.5"))


if __name__ == "__main__":
    app = QtWidgets.QApplication(sys.argv)
    MainWindow = QtWidgets.QMainWindow()
    ui = Ui_Form()
    ui.setupUi(MainWindow)
    MainWindow.show()
    sys.exit(app.exec_())
