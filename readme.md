FastCloudflare - CloudflareST的Windows GUI衍生版

简介

FastCloudflare 是一个基于 CloudflareST 的 Windows GUI 应用程序，旨在帮助用户一键获取 Cloudflare 的最优 IP 地址，并自动将其添加到系统的 hosts 文件中。这可以帮助解决访问 Cloudflare 服务时遇到的一些问题，如官网人机认证无法打开、CDN 文件超时和带宽小等问题。FastCloudflare 提供了一个简单易用的图形界面，使得小白用户也能轻松操作。

功能

- 一键优选：程序会自动运行 CloudflareST.exe 来获取最优 IP 地址。
- 自动修改 hosts：获取到最优 IP 后，程序会自动修改系统的 hosts 文件，将最优 IP 地址添加到 Cloudflare 的相关域名。
- 备份 hosts 文件：在修改 hosts 文件之前，程序会自动备份原始的 hosts 文件。
- 清空 DNS 缓存：修改 hosts 文件后，程序会自动清空 DNS 缓存，确保新的 IP 地址立即生效。
- 管理员权限：程序需要以管理员权限运行，以确保能够修改 hosts 文件和清空 DNS 缓存。

使用方法

1. 下载程序：从 [Releases](https://github.com/yourusername/fastcloudflare/releases) 页面下载最新版本的 FastCloudflare。
2. 解压文件：将下载的压缩包解压到本地文件夹。
3. 以管理员权限运行：右键点击 `fastcf.exe` 文件，选择“以管理员身份运行”。
4. 点击启动：在程序界面中，点击带有 Cloudflare 标志的按钮启动优选程序。
5. 查看日志：程序会显示详细的运行日志，包括获取最优 IP 的过程和修改 hosts 文件的结果。
6. 完成：程序完成后，会弹出提示框告知用户操作已完成。

注意事项

- 请确保在运行程序之前关闭所有正在使用 Cloudflare 服务的应用程序。
- 如果在使用过程中遇到任何问题，请查看程序的日志输出，或在 Issues 中提交问题。
- 本程序仅适用于 Windows 操作系统。

贡献

如果你对 FastCloudflare 有任何改进建议或发现 bug，欢迎提交 Pull Requests 或 Issues。

许可证

FastCloudflare 是开源软件，遵循 [MIT 许可证](https://opensource.org/licenses/MIT)。你可以自由地使用、修改和分发本程序，但请保留原作者的版权信息。

---

FastCloudflare 作为 CloudflareST 的 Windows GUI 版本，旨在为不熟悉命令行操作的用户提供便利。希望这个 README 文件能够帮助你更好地了解和使用 FastCloudflare。如果你喜欢这个项目，请给我一个 Star 支持！🌟