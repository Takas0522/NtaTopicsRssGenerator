# このリポジトリについて

国税庁の[トピックス一覧](https://www.nta.go.jp/information/news/index.htm)をRSS化するツールです。

RSSのXMLは<s>[こちら]()</s>（準備中）に格納しています。

# 使用している技術について

* Azure Functions
* .NET6（C#）

# ローカルでの実行の仕方

* azuriteで起動したエミュレータのBLOB Storageに`rss-feed`という名前でコンテナを作成します
   * 後述の設定を行うことでコンテナ名は自由に変更可能です
* `local.settings.dummy.json`を`local.settings.json`にリネームすれば実行できます。
* 実行後、コンテナの中に`rss.xml`という名前でファイルが生成されます
   * 後述の設定を行うことでファイル名は自由に変更可能です

# 環境変数(local.settings.json)に設定する内容について

それぞれ既定値があるため設定しなくても動作します。

* NTA_ORIGIN: 国税庁のHPのURLのoriginを設定します（既定値：https://www.nta.go.jp）
* NTA_ENDPOINT: 国税庁のトピックス一覧のエンドポイントを設定します※Origin部分除く（既定値：/information/news/index.htm）
* CONTAINER_NAME: Azure BLOB Storageの格納するコンテナ名を設定します（既定値：rss-feed）
* BLOB_NAME: 格納するRSS XMLのファイル名を設定します（既定値：rss.xml）