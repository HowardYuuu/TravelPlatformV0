    var root = am5.Root.new("chartdiv");
    root.setThemes([am5themes_Animated.new(root)]);
    var chart = root.container.children.push(am5map.MapChart.new(root, {
        panX: "rotateX",
        panY: "translateY",
        projection: am5map.geoNaturalEarth1(),
        rotationX: -5
    })),
        polygonSeries = chart.series.push(am5map.MapPolygonSeries.new(root, {
            geoJSON: am5geodata_worldLow,
            geodataNames: am5geodata_lang_tw_ZH
        })),
        o = am5.ColorSet.new(root, {}),
        n = 0;
    polygonSeries.mapPolygons.template.adapters.add("fill", function (e, t) {
        n < 10 ? n++ : n = 0;
        var a = t.dataItem.dataContext;
        if (a.colorWasSet) return e;
        a.colorWasSet = !0;
        var l = am5.Color.saturate(o.getIndex(6 * n), .38);
        return t.setRaw("fill", l), l
    });
    var l = chart.children.push(am5.Legend.new(root, {
        useDefaultMarker: !0,
        centerX: am5.p50,
        x: am5.p50,
        centerY: am5.p100,
        y: am5.p100,
        dy: -20,
        background: am5.RoundedRectangle.new(root, {
            fill: am5.color(16777215),
            fillOpacity: .2
        })
    }));
    l.valueLabels.template.set("forceHidden", !0);
    var r = am5.ColorSet.new(root, {
        step: 77
    });
    r.next(), polygonSeries.mapPolygons.template.states.create("hover", {
        fillOpacity: 1
    }), (async () => {
        const loadpic = document.querySelector('.load-6')
        loadpic.style.display = 'block';
        let a = await getProductCount();
        let isEmbassy = await GetEmbassies();
        if (!isEmbassy) {
            return;
        }
        loadpic.style.display = 'none';
        !0 === a ? am5.array.each(groupData, function (a) {
            var o = [],
                n = r.next();
            am5.array.each(a.data, function (e) {
                o.push(e.id)
            });
            var i = chart.series.push(am5map.MapPolygonSeries.new(root, {
                geoJSON: am5geodata_worldLow,
                include: o,
                name: a.name,
                fill: n,
                fill: am5.Color.brighten(n, -.3)
            }));
            i.mapPolygons.template.setAll({
                tooltipText: "{name}[/]\n{detail}",
                interactive: !0,
                fill: n,
                strokeWidth: .5
            }), i.mapPolygons.template.states.create("hover", {
                fill: am5.Color.brighten(n, -.45)
            }), i.mapPolygons.template.events.on("click", e => {
                var t = e.target.dataItem.dataContext;


                if (t.Url) {
                    window.open(t.Url, '_blank');
                }

                if (t.detail) {
                    if (t.detail.includes('商品數量')) {
                        const filterLink = document.querySelector('.filterLink');
                        filterLink.addEventListener('click', SaveCountryLocalStorage(t.name));
                        SaveCountryLocalStorage(t.name);
                        filterLink.click();
                    }
                }




            }), i.mapPolygons.template.events.on("pointerout", function (e) {
                e.target.series.mapPolygons.each(function (e) {
                    e.states.applyAnimate("default")
                })
            }), i.data.setAll(a.data), l.data.push(i)
        }) : console.log("!")
    })(), polygonSeries.mapPolygons.template.setAll({
        tooltipText: "{name}",
        fillOpacity: .8
    }), polygonSeries.mapPolygons.template.events.on("click", e => {
        var t = e.target.dataItem.dataContext;



        //console.log(t.name), console.log(t.detail)
        //if (t.detail) {
        //    if (t.detail.includes('商品數量')) {
        //        const filterLink = document.querySelector('.filterLink');
        //        filterLink.addEventListener('click', SaveCountryLocalStorage(t.name));
        //        SaveCountryLocalStorage(t.name);
        //        filterLink.click();
        //    }
        //}




    }), chart.series.push(am5map.GraticuleSeries.new(root, {})).mapLines.template.setAll({
        stroke: root.interfaceColors.get("alternativeBackground"),
        strokeOpacity: .04
    }), chart.set("zoomControl", am5map.ZoomControl.new(root, {})), chart.chartContainer.get("background").events.on("click", function () {
        chart.goHome()
    });
    var i = chart.series.unshift(am5map.MapPolygonSeries.new(root, {}));
    i.mapPolygons.template.setAll({
        fill: am5.color(15595514),
        stroke: am5.color(15595514)
    }), i.data.push({
        geometry: am5map.getGeoRectangle(90, 180, -90, -180)
    });
    var p = chart.children.push(am5.Container.new(root, {
        x: am5.p100,
        dx: -13,
        centerX: am5.p100,
        y: am5.p100,
        dy: -110,
        centerY: am5.p100,
        layout: root.verticalLayout,
        paddingTop: 0,
        paddingRight: 0,
        paddingBottom: 0,
        paddingLeft: 0,
        background: am5.RoundedRectangle.new(root, {
            fill: am5.color(16777215),
            fillOpacity: .3
        })
    }));

    function s(a, o, n, l) {
        p.children.push(am5.Button.new(root, {
            paddingTop: 0,
            paddingRight: 0,
            paddingBottom: 0,
            paddingLeft: 0,
            marginTop: 3,
            marginLeft: 5,
            marginRight: 5,
            label: am5.Label.new(root, {
                text: a
            })
        })).events.on("click", function () {
            chart.setAll({
                projection: o,
                panY: n,
                rotationY: l
            }), chart.goHome()
        })
    }
    s("1", am5map.geoNaturalEarth1(), "translateY", 0), s("2", am5map.geoMercator(), "none", 0), s("3", am5map.geoEquirectangular(), "none", 0), s("4", am5map.geoOrthographic(), "rotateY", 0)

    var previousPolygon;

    polygonSeries.mapPolygons.template.on("active", function (active, target) {
        if (previousPolygon && previousPolygon != target) {
            previousPolygon.set("active", false);
        }
        if (target.get("active")) {
            polygonSeries.zoomToDataItem(target.dataItem);
        }
        else {
            chart.goHome();
        }
        previousPolygon = target;
    });


    chart.appear(1e3, 100)