import os
import pathlib
import shutil
import subprocess
import threading
import time
import tkinter as tk
from tkinter import ttk, filedialog, messagebox
import sv_ttk
from globals import *

def init_createProj_ui(root):
    title_label = ttk.Label(root, text="Create New Project", font=("Segoe UI", 18, "bold"))
    title_label.pack(anchor=tk.W, padx=15, pady=15)

    bottom_frame = ttk.Frame(root)
    bottom_frame.pack(side=tk.BOTTOM, fill=tk.X, padx=15, pady=15)

    entry_frame = ttk.Frame(root)
    entry_frame.pack()

    name_label = ttk.Label(entry_frame, text="Project Name:")
    name_label.pack(anchor=tk.W, padx=15, pady=5)

    name_entry = ttk.Entry(entry_frame, width=50)
    name_entry.pack(anchor=tk.W, padx=15, pady=5)

    path_label = ttk.Label(entry_frame, text="Project Path:")
    path_label.pack(anchor=tk.W, padx=15, pady=5)

    path_frame = ttk.Frame(entry_frame)
    path_frame.pack(anchor=tk.W, padx=15, pady=5)

    path_entry = ttk.Entry(path_frame, width=40)
    path_entry.pack(side=tk.LEFT)

    def browse_path():
        sv_ttk.use_light_theme()
        root.withdraw()

        root.after(100, lambda: (
            path := filedialog.askdirectory(parent=root, title='Select Project Path', initialdir='/home/preslav/Projects/', mustexist=True), 
            path_entry.delete(0, tk.END) if path else None, 
            path_entry.insert(0, path) if path else None, 
            
            sv_ttk.set_theme("dark"),
            root.deiconify()
        ))
    
    browse_button = ttk.Button(path_frame, text="Browse", command=browse_path)
    browse_button.pack(side=tk.LEFT, padx=(5, 0))

    version_label = ttk.Label(bottom_frame, text=f"version: {VERSION}", foreground="gray", font=("Segoe UI", 10))
    version_label.pack(side=tk.LEFT, anchor=tk.SW)

    button_frame = ttk.Frame(bottom_frame)
    button_frame.pack(side=tk.RIGHT, anchor=tk.SE)

    cancel_button = ttk.Button(button_frame, text="Cancel", command=root.quit)
    cancel_button.pack(side=tk.LEFT)

    progress_frame = ttk.Frame(root)
    progress_label = ttk.Label(progress_frame, text="Starting creation...", font=("Segoe UI", 12))
    progress_label.pack(padx=15, pady=10)
    
    progress_bar = ttk.Progressbar(progress_frame, mode="determinate")
    progress_bar.pack(padx=15, pady=10, fill=tk.X)

    def create_project(path, name):
        title_label.pack_forget()
        bottom_frame.pack_forget()
        entry_frame.pack_forget()

        progress_frame.place(anchor=tk.CENTER, relx=0.5, rely=0.5)

        def rename(old_name, new_name, proj_path):
            old_folder_path = proj_path / old_name

            if old_folder_path.exists() and old_folder_path.is_dir():
                new_folder_path = proj_path / new_name
                old_folder_path.rename(new_folder_path)
                print(f"Renamed folder from {old_folder_path} to {new_folder_path}")
            else:
                print(f"Folder {old_folder_path} does not exist.")

        def rename_file(old_name, new_name, folder_path):
            old_file_path = folder_path / old_name

            if old_file_path.exists() and old_file_path.is_file():
                new_file_path = folder_path / new_name
                old_file_path.rename(new_file_path)
                print(f"Renamed file from {old_file_path} to {new_file_path}")
            else:
                print(f"File {old_file_path} does not exist or is not a file.")

        def run_creation():
            try:
                dir_path = pathlib.Path(path)
                project_name = name

                progress_label.config(text="Creating directory...")
                progress_bar.config(value=10)
                time.sleep(0.05)
                project_path = dir_path / project_name
                project_path.mkdir(parents=True, exist_ok=True)

                progress_label.config(text="Copying template...")
                progress_bar.config(value=35)
                time.sleep(0.1)
                template_path = pathlib.Path("TEMPLATE/raw/")
                shutil.copytree(template_path, project_path, dirs_exist_ok=True)

                progress_label.config(text="Configuring project...")
                progress_bar.config(value=75)
                time.sleep(0.1)
                rename("BraketsRaw-Template", f"{project_name}_project", project_path)
                rename("BraketsTemplate", project_name, project_path / f"{project_name}_project")
                rename_file("BraketsTemplate.sln", f"{project_name}.sln", project_path / f"{project_name}_project")
                rename_file("BraketsTemplate.csproj", f"{project_name}.csproj", project_path / f"{project_name}_project" / f"{project_name}")

                # Change .sln contents
                with open(project_path / f"{project_name}_project" / f"{project_name}.sln", 'r') as file:
                    content = file.read()

                updated_content = content.replace("BraketsTemplate", project_name)

                with open(project_path / f"{project_name}_project" / f"{project_name}.sln", 'w') as file:
                    file.write(updated_content)

                progress_label.config(text="Project created!")
                progress_bar.config(value=85)
                print(f"Created new project: {project_path}")
                time.sleep(0.15)

                with open("DATA/PROJECT_PATHS.txt", "a") as file:
                    file.write(f"{project_path}")

                progress_label.config(text="Saved project path!")
                progress_bar.config(value=100)
                time.sleep(0.1)

                progress_label.config(text="Initializing Editor...")
                
                progress_bar.config(mode="indeterminate")
                progress_bar.start()
                print("Initializing editor...")
                
                time.sleep(1)
                progress_bar.stop()
                root.withdraw()

                os.system(f"./../BraketsEditor/BraketsEditor {project_name} {project_path}/{project_name}_project/{project_name}/")

            except Exception as e:
                print(f"FAILED TO CREATE PROJECT! Error: {e}")
                result = messagebox.askretrycancel("Failed", "Failed to open project!\nEX:{e}")
                if result == messagebox.RETRY:
                    create_project(path, name)
                else:
                    return
            finally:
                print("Exit")
                root.quit()
                exit()

        threading.Thread(target=run_creation).start()

    create_button = ttk.Button(button_frame, text="Create", style='Accent.TButton', width=15, command=lambda: create_project(path_entry.get(), name_entry.get()))
    create_button.pack(side=tk.LEFT, padx=(10, 0))