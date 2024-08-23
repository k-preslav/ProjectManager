import argparse
import tkinter as tk
from tkinter import ttk
import sv_ttk

from createProject import init_createProj_ui
from openProject import init_openProject_ui

def main(start_mode):
    root = tk.Tk()
    root.title("Brakets Project Manager")

    window_size = (450, 300)

    # UI
    if start_mode == "new":
        window_size = (450, 300)
        root.resizable(False, False)
        init_createProj_ui(root)
    elif start_mode == "open":
        window_size = (800, 600)
        init_openProject_ui(root)

    root.geometry(f"{window_size[0]}x{window_size[1]}")

    screen_width = root.winfo_screenwidth()
    screen_height = root.winfo_screenheight()
    x = (screen_width / 2) - (window_size[0] / 2)
    y = (screen_height / 2) - (window_size[1] / 2)
    root.geometry(f"+{int(x)}+{int(y)}")
    root.minsize(window_size[0], window_size[1])
    
    sv_ttk.use_dark_theme()
    root.mainloop()

if __name__ == "__main__":
    parser = argparse.ArgumentParser()
    parser.add_argument("--start", help="start mode: new/open")

    args = parser.parse_args()

    if args.start == "new":
        main("new")
    elif args.start == "open":
        main("open")
    else:
        print("Invalid start mode!")