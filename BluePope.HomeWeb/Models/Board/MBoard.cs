﻿using BluePope.Lib.DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BluePope.HomeWeb.Models.Board
{
    #region "Home.BOARD 테이블"
    /*
CREATE TABLE `BOARD` (
	`SEQ` INT(11) NOT NULL AUTO_INCREMENT,
	`BOARD_TYPE` VARCHAR(50) NOT NULL COLLATE 'utf8mb4_unicode_ci',
	`TITLE` VARCHAR(200) NOT NULL COLLATE 'utf8mb4_unicode_ci',
	`CONTENTS` MEDIUMTEXT NOT NULL COLLATE 'utf8mb4_unicode_ci',
	`DUP_KEY` BIGINT(20) NOT NULL,
	`VIEW_CNT` INT(11) NOT NULL,
	`STATUS_FLAG` VARCHAR(50) NOT NULL COLLATE 'utf8mb4_unicode_ci',
	`REG_IP` VARCHAR(20) NOT NULL COLLATE 'utf8mb4_unicode_ci',
	`REG_USER` VARCHAR(50) NOT NULL COLLATE 'utf8mb4_unicode_ci',
	`REG_USERNAME` VARCHAR(50) NOT NULL COLLATE 'utf8mb4_unicode_ci',
	`REG_DATE` DATETIME NOT NULL,
	PRIMARY KEY (`SEQ`)
)
COLLATE='utf8mb4_unicode_ci'
ENGINE=InnoDB
AUTO_INCREMENT=2
;
    */
    #endregion

    public class MBoard
    {
        /// <summary>
        /// 중복 입력 방지용
        /// </summary>
        public string DUP_KEY { get; set; }

        public string BOARD_TYPE { get; set; }
        public int SEQ { get; set; }
        public string TITLE { get; set; }
        public string CONTENTS { get; set; }
        public int VIEW_CNT { get; set; }
        public string STATUS_FLAG { get; set; }

        public string REG_IP { get; set; }
        public string REG_USER { get; set; }
        public string REG_USERNAME { get; set; }
        public DateTime? REG_DATE { get; set; }

        /* 필요한가? 
        public string UPDATE_IP { get; set; }
        public string UPDATE_USER { get; set; }
        public string UPDATE_USERNAME { get; set; }
        public DateTime? UPDATE_DATE { get; set; }
        */

        public static async Task<MBoard> Get(string board_type, int seq)
        {
            return (await MySqlDapperHelper.Instance.GetQueryFromXmlAsync<MBoard>("Sql/Board.xml", "GetBoard", new
            {
                board_type = board_type,
                seq = seq
            })).FirstOrDefault();
        }

        public static async Task<MBoard> GetAsync(string board_type, int seq)
        {
            var model = await MySqlDapperHelper.Instance.GetQueryFromXmlAsync<MBoard>("Sql/Board.xml", "GetBoard", new
            {
                board_type = board_type,
                seq = seq
            });
            
            return model.FirstOrDefault();
        }

        public static async Task<int> GetCount(string board_type)
        {
            return (await MySqlDapperHelper.Instance.GetQueryFromXmlAsync<int>("Sql/Board.xml", "GetBoardCount", new
            {
                board_type = board_type,
            })).FirstOrDefault();
            
        }

        public static async Task<IEnumerable<MBoard>> GetList(string board_type, int page = 1, int page_size = 20)
        {
            //데이터가 많아지면 LIMIT가 느려질수 있다고함, WHERE 로 모집합을 줄이고 LIMIT를 걸어야한다고....
            var sql = MySqlDapperHelper.GetSqlFromXml("Sql/Board.xml", "GetBoardList");
            var limit = $" LIMIT {(page - 1) * page_size}, {page_size}";

            return await MySqlDapperHelper.Instance.GetQueryAsync<MBoard>(sql + limit, new
            {
                board_type = board_type,
            });
        }

        public static async Task<IEnumerable<MBoard>> GetListAsync(string board_type, int page = 1, int page_size = 20)
        {
            //데이터가 많아지면 LIMIT가 느려질수 있다고함, WHERE 로 모집합을 줄이고 LIMIT를 걸어야한다고....
            var sql = MySqlDapperHelper.GetSqlFromXml("Sql/Board.xml", "GetBoardList");
            var limit = $" LIMIT {(page - 1) * page_size}, {page_size}";

            return await MySqlDapperHelper.Instance.GetQueryAsync<MBoard>(sql + limit, new
            {
                board_type = board_type,
            });
        }

        public async Task<int> AddViewCount()
        {
            return await MySqlDapperHelper.Instance.ExecuteFromXmlAsync("Sql/Board.xml", "UpdateViewCount", this);
        }

        public async Task<int> Insert(MySqlDapperHelper db)
        {
            return await db.ExecuteFromXmlAsync("Sql/Board.xml", "InsertBoard", this);
        }
    }
}