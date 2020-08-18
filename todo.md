開発は終了しているのToDoもなにもありません。

## プラットフォーム
- [x] Android (Xamarin Forms。ほぼ完成)
- [ ] ~~iOS (Macとライセンスがあれば比較的容易に移植可能)~~
- [ ] Windows UWP (Xamarin Formsではなくネイティブ開発~~予定~~ )(予定なし)
- [ ] Windows Desktop (普通のデスクトップアプリ。wpf。)(予定なし)

## ToDo
- [x] テスト成績一覧
- [x] テスト後成績
- [x] ストア
- [x] Amazon検索
- [x] アプリ情報 (ライセンス) (設定内で済ます。)
- [x] オープンソースライセンス
- [x] 作者情報 (その為だけにわざわざTwitter APIを組み込んだ。)
- [x] チュートリアル
- [x] 設定画面
- [x] アプリアイコン設定 (仮)
- [x] 単語帳削除
- [x] 単語帳検索
- [x] ソート
- [x] 全体検索
- [x] 全体テスト
- [x] ツールバーアイコン作成
- [x] csvインポート
- [ ] ~~csvエクスポート~~ 対応せず。
- [x] xlsxインポート
- [ ] ~~xlsxエクスポート~~ 対応せず。
- [ ] ~~EBStudioエクスポート~~ 対応せず。
- [x] クイズにプログレスバー
- [x] ツイートに宣伝機能 (Google PlayのURLはまだわからん。)
- [x] ツイートに全問正解
- [x] 単語帳別成績
- [x] カレンダー (Github風)
- [x] 単語帳の更新機能
- [x] 単語帳の更新同期
- [x] 成績に開始日時と種類
- [ ] ~~単語帳をタブ式に(成績一覧を表示・ボタンにするべきか？)~~ ボタンにした。
- [x] 単語帳名の拡張
- [x] 単語帳名のサジェスト
- [x] ~~ボタンを青色とかに~~。青色は微妙だったが、灰色も微妙。
- [x] 再試験
- [x] ダミー単語帳アイコン
- [x] ICND1模擬問等に対応
- [x] 国際化 (日英。面倒だが単なる作業。自然な訳か気になる。)
- [x] 正解数でスキップに試験数も条件に加える
- [x] サジェスト付きエントリーUI (エラー表示も欲しい)
- [x] 最終出題日時
- [x] ~~ペンアイコン~~
- [ ] ストア用アセットなど様々
- [ ] 画像読み込み制限があるっぽいので確認
- [x] 海外用チュートリアル画像
- [x] Storage周りのマルチスレッド対応が不十分。
- [x] 謎のObjectDisposed Exception. (Storage周りをマシにしてから見てない。原因はやはり不明。)

```
Cannot access a disposed object.
Object name: 'Android.Graphics.Bitmap'.
```
つまりAndroidが画像を勝手にDisposeしてるらしい。
例外出さずに再読み込みしてくれればいいのに。
CacheStrategy="RecycleElement"で治るらしい。
[参照](https://forums.xamarin.com/discussion/79315/xamarin-forms-bug-system-objectdisposedexception-cannot-access-a-disposed-object)
