<script lang="tsx" name="LayoutFooter">
import { defineComponent } from "vue";
import { useSiteConfig } from "@/stores/siteConfig";

export default defineComponent({
    name: "LayoutFooter",
    render() {
        const siteConfigStore = useSiteConfig();

        return (
            <el-footer class="footer">
                <div>
                    Copyright
                    {siteConfigStore.state.copyrightValidStartYear !== null && <> @ {siteConfigStore.state.copyrightValidStartYear}</>}
                    {siteConfigStore.state.copyrightValidEndYear !== null && <>~{siteConfigStore.state.copyrightValidEndYear} </>}
                    {siteConfigStore.state.copyrighted && (
                        <>
                            {" "}
                            {siteConfigStore.state.copyrightedUrl ? (
                                <a href={siteConfigStore.state.copyrightedUrl}>{siteConfigStore.state.copyrighted}</a>
                            ) : (
                                <>{siteConfigStore.state.copyrighted}</>
                            )}{" "}
                        </>
                    )}
                    {siteConfigStore.state.version && <> {siteConfigStore.state.version} </>}
                    {siteConfigStore.state.icpInfo && (
                        <>
                            {" "}
                            <a href="https://beian.miit.gov.cn/">{siteConfigStore.state.icpInfo}</a>{" "}
                        </>
                    )}
                    {siteConfigStore.state.publicProvince && siteConfigStore.state.publicCode && (
                        <>
                            {" "}
                            <img src="/src/assets/publicLogo.png" />{" "}
                            <a href={`https://beian.mps.gov.cn/#/query/webSearch?code=${siteConfigStore.state.publicCode}`}>
                                {siteConfigStore.state.publicProvince}
                                {siteConfigStore.state.publicCode}号
                            </a>{" "}
                        </>
                    )}
                </div>
            </el-footer>
        );
    },
});
</script>

<style scoped lang="scss">
.footer {
    display: flex;
    width: 100%;
    justify-content: center;
    align-items: center;
    height: 30px;
    background-color: var(--fast-bg-color);
    margin: 5px 0;

    a {
        font-size: 14px;
        letter-spacing: 0.5px;
        text-decoration: none;
        color: var(--el-text-color-secondary);
    }

    img {
        width: 16px;
        vertical-align: top;
    }
}
</style>
