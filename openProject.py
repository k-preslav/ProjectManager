import os
import threading
import time
import tkinter as tk
from tkinter import ttk, messagebox
from globals import *

def init_openProject_ui(root):
    is_getting_data = False
    
    title_frame = ttk.Frame(root)
    title_frame.pack(fill=tk.X, anchor=tk.E)

    menubutton = ttk.Button(title_frame, text="☰", style='Menubutton.TButton', command=lambda: print("Manage installation"))
    menubutton.pack(side=tk.RIGHT, padx=(0, 15), pady=15)

    refreshbutton = ttk.Button(title_frame, text="⟳", style='Menubutton.TButton', command=lambda: threading.Thread(target=get_data).start())
    refreshbutton.pack(side=tk.RIGHT, padx=5, pady=15)

    title_label = ttk.Label(title_frame, text="Open Project", font=("Segoe UI", 18, "bold"))
    title_label.pack(side=tk.LEFT, anchor=tk.E, padx=15, pady=15)

    progress_frame = ttk.Frame(root)

    progress_bar = ttk.Progressbar(progress_frame, orient="horizontal", length=250, mode="determinate")
    progress_bar.pack()

    progress_text = ttk.Label(progress_frame, text="PROGRESS")
    progress_text.pack(pady=20)

    list_frame = ttk.Frame(root)
    list_frame.pack(fill=tk.BOTH, expand=True, padx=15, pady=5)

    project_list = ttk.Treeview(list_frame, columns=("Name", "Path", "Engine Version"), show="headings")
    project_list.pack(fill=tk.BOTH, expand=True)

    project_list.heading("Name", text="Name")
    project_list.heading("Path", text="Path")
    project_list.heading("Engine Version", text="Engine Version")

    project_list.column("Name", width=75, stretch=tk.YES)
    project_list.column("Path", width=250, stretch=tk.YES)
    project_list.column("Engine Version", width=120, stretch=tk.NO)

    # Get the project data
    def get_data():
        nonlocal is_getting_data

        if is_getting_data:
            return
        
        is_getting_data = True

        print("Starting refresh")
        project_list.delete(*project_list.get_children())
        list_frame.pack_forget()

        progress_frame.place(anchor=tk.CENTER, relx=0.5, rely=0.5)
        
        with open("DATA/PROJECT_PATHS.txt", "r") as file:
            paths = file.read().split("\n")
        
        progress_bar["maximum"] = len(paths)
        
        for index, path in enumerate(paths):
            if path.strip():
                name = os.path.basename(path)
                try:
                    with open(f"{path}/{name}_project/{name}/game.properties") as propFile:
                        properties = propFile.read().split("\n")
                        engine_ver = None
                        for line in properties:
                            if line.strip():
                                key, value = line.split(",")
                                if key == "engine_ver":
                                    engine_ver = value

                    project_list.insert("", "end", values=(name, path, engine_ver))
                except Exception as e:
                    print(f"Error reading {path}: {e}")

            progress_bar["value"] = index + 1
            progress_text["text"] = f"Indexing projects: {index}/{len(paths)}"
            root.update_idletasks()

        progress_frame.pack_forget()
        list_frame.pack(fill=tk.BOTH, expand=True, padx=15, pady=5)

        is_getting_data = False
        print("Refresh complete.")

    bottom_frame = tk.Frame(root)
    bottom_frame.pack(side=tk.BOTTOM, fill=tk.X, padx=15, pady=15)

    version_label = ttk.Label(bottom_frame, text=f"version: {VERSION}", foreground="gray", font=("Segoe UI", 10))
    version_label.pack(side=tk.LEFT, anchor=tk.SW)

    def get_selected():
        selected_item = project_list.selection()
    
        if selected_item:
            item_id = selected_item[0]
            item_values = project_list.item(item_id)['values']
            
            name = item_values[0]
            path = item_values[1]
            engine_version = item_values[2]

            return name, path, engine_version
        
    def open_project():
        try:
            name, path, engine_ver = get_selected()

            progress_bar.config(mode="indeterminate")

            progress_frame.pack(pady=20)
            progress_text.config(text=f"Opening project: {name}")

            progress_bar.start()

            time.sleep(1)

            root.withdraw()
            progress_bar.stop()
            
            os.system(f"./../BraketsEditor/BraketsEditor {name} {path}/{name}_project/{name}/")
        except Exception as e:
            print(f"FAILED TO OPEN PROJECT! Error: {e}")
            result = messagebox.askretrycancel("Failed", "Failed to open project!\nEX:{e}")
            if result == messagebox.RETRY:
                open_project()
            else:
                return
        finally:
            print("Exit")
            root.quit()
            exit()

    open_button = ttk.Button(bottom_frame, text="Open", style='Accent.TButton', width=10, command=lambda: threading.Thread(target=open_project).start())
    open_button.pack(side=tk.RIGHT, anchor=tk.SE)
    
    manage_button = ttk.Button(bottom_frame, text="Manage Project", width=15)
    manage_button.pack(side=tk.RIGHT, anchor=tk.SE, padx=(0, 10))

    threading.Thread(target=get_data).start()