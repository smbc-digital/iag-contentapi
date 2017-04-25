import os
import sys
from  zipfile import ZipFile


def zip_file(srcDir, dest):
    print("Zipping src dir '{0}' to '{1}'".format(srcDir, dest))
    abs_src = os.path.abspath(srcDir)
    with ZipFile(dest, 'w') as config_zip:
        for root, dirs, files in os.walk(srcDir):
            for file in files:
                absname = os.path.abspath(os.path.join(root, file))
                arcname = absname[len(abs_src) + 1:]
                print("Zipping '{0}' as '{1}'".format((os.path.join(root, file), arcname)))
                config_zip.write(absname, arcname)

if __name__ == "__main__":
    zip_file(sys.argv[1], sys.argv[2])
